using Carma.Application.Abstractions;
using Carma.Application.Common;
using Carma.Application.DTOs.Review;
using Carma.Application.DTOs.User;
using Carma.Domain.Entities;
using Carma.Domain.Enums;
using Carma.Domain.Factories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Carma.Application.Services;

public class ReviewService
{
    private readonly ICarmaDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<ReviewCreateDto> _createValidator;

    public ReviewService(ICarmaDbContext context, ICurrentUserService currentUserService, IValidator<ReviewCreateDto> createValidator)
    {
        _context = context;
        _currentUserService = currentUserService;
        _createValidator = createValidator;
    }

    public async Task<Result<IEnumerable<ReviewGetDto>>> GetAllForUserAsync(Guid userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            return Result<IEnumerable<ReviewGetDto>>.NotFound("User not found");
        }
        
        var reviews = await _context.Reviews
            .AsNoTracking()
            .Where(r => r.ReviewedUserId == userId)
            .Select(r => new ReviewGetDto(
                    new UserSummaryDto(
                        r.Reviewer.Id,
                        r.Reviewer.UserName!,
                        r.Reviewer.ImageUrl,
                        r.Reviewer.Karma
                        ),
                    r.RideId,
                    r.Karma,
                    r.Text,
                    r.CreatedAt
                )
            )
            .ToListAsync();
        return Result<IEnumerable<ReviewGetDto>>.Success(reviews);
    }

    public async Task<Result<ReviewGetDto>> AddReviewAsync(Guid userId, ReviewCreateDto reviewCreateDto)
    {
        var validationResult = await _createValidator.ValidateAsync(reviewCreateDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result<ReviewGetDto>.Failure(string.Join("; ", errors));
        }
        
        var currentUserId = _currentUserService.UserId;
        var ride = await _context.Rides
            .Include(r => r.Participants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(r => r.Id == reviewCreateDto.RideId);
        if (ride == null)
        {
            return Result<ReviewGetDto>.NotFound("Ride not found");
        }

        if (ride.Status != Status.Completed)
        {
            return Result<ReviewGetDto>.Conflict("You cannot review a ride that is not completed");
        }
        
        var reviewer = ride.Participants.FirstOrDefault(p => p.UserId == currentUserId && p.Status == ParticipantStatus.Accepted);
        var target = ride.Participants.FirstOrDefault(p => p.UserId == userId && p.Status == ParticipantStatus.Accepted);

        if (reviewer == null)
        {
            return Result<ReviewGetDto>.Unauthorized("You can only review users from rides you participated in");
        }

        if (target == null)
        {
            return Result<ReviewGetDto>.Conflict("You cannot review a participant that is not in the ride");
        }
        
        if (target.UserId == currentUserId)
        {
            return Result<ReviewGetDto>.Conflict("You cannot review yourself");
        }

        var alreadyReviewed = ride.Reviews.Any(r => 
            r.ReviewerId == currentUserId && r.ReviewedUserId == userId);

        if (alreadyReviewed)
        {
            return Result<ReviewGetDto>.Conflict(
                "You have already reviewed this user for this ride"
            );
        }
        
        var targetUser = target.User;
        
        targetUser.Karma = (targetUser.Karma * targetUser.ReviewsCount + reviewCreateDto.Karma) / (targetUser.ReviewsCount + 1);
        targetUser.ReviewsCount++;
        
        var review = new Review
        {
            ReviewerId = currentUserId,
            ReviewedUserId = userId,
            RideId = reviewCreateDto.RideId,
            Karma = reviewCreateDto.Karma,
            Text = reviewCreateDto.Text,
            CreatedAt = DateTime.UtcNow
        };
        
        await _context.Reviews.AddAsync(review);
        
        await _context.Notifications.AddAsync(NotificationFactory.CreateReview(userId, reviewCreateDto.RideId, _currentUserService.Username));
        
        await _context.SaveChangesAsync();
        
        return Result<ReviewGetDto>.Success(new ReviewGetDto(
                new UserSummaryDto(
                    targetUser.Id,
                    targetUser.UserName ?? string.Empty,
                    targetUser.ImageUrl,
                    targetUser.Karma
                ),
            reviewCreateDto.RideId,
            reviewCreateDto.Karma,
            reviewCreateDto.Text,
            DateTime.UtcNow
            )
        );
    }
}