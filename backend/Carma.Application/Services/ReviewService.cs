using Carma.Application.Abstractions;
using Carma.Application.Common;
using Carma.Application.DTOs.Review;
using Carma.Application.DTOs.User;
using Carma.Domain.Entities;
using Carma.Domain.Enums;
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
            .Where(r => r.ReviewedUserId == userId)
            .Select(r => new ReviewGetDto(
                    new UserSummaryDto(
                        r.Reviewer.Id,
                        r.Reviewer.UserName,
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
            return Result<ReviewGetDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).First());
        }
        
        var currentUserId = _currentUserService.UserId;
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return Result<ReviewGetDto>.NotFound("User not found");
        }

        if (user.Id == currentUserId)
        {
            return Result<ReviewGetDto>.Conflict("You cannot review yourself");
        }
        
        var ride = await _context.Rides.FindAsync(reviewCreateDto.RideId);
        if (ride == null)
        {
            return Result<ReviewGetDto>.NotFound("Ride not found");
        }

        if (ride.Status != Status.Completed)
        {
            return Result<ReviewGetDto>.Conflict("You cannot review a ride that is not completed");
        }

        var reviewerIsInRide =
            await _context.RideParticipants.AnyAsync(rp =>
                rp.UserId == currentUserId && 
                rp.RideId == reviewCreateDto.RideId && 
                rp.Status == ParticipantStatus.Accepted);
        if (!reviewerIsInRide)
        {
            return Result<ReviewGetDto>.Unauthorized("You can only review users from rides you participated in");
        }
        
        var reviewedIsInRide =
            await _context.RideParticipants.AnyAsync(rp => 
                rp.UserId == userId && 
                rp.RideId == reviewCreateDto.RideId && 
                rp.Status == ParticipantStatus.Accepted);
        if (!reviewedIsInRide)
        {
            return Result<ReviewGetDto>.Conflict("You cannot review a participant that is not in the ride");
        }

        var alreadyReviewed = 
            await _context.Reviews.AnyAsync(r => 
                r.ReviewerId == currentUserId && 
                r.ReviewedUserId == userId && 
                r.RideId == reviewCreateDto.RideId);

        if (alreadyReviewed)
        {
            return Result<ReviewGetDto>.Conflict(
                "You have already reviewed this user for this ride"
            );
        }
        
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
        
        user.Karma = (user.Karma * user.ReviewsCount + reviewCreateDto.Karma) / (user.ReviewsCount + 1);
        user.ReviewsCount++;

        await _context.Notifications.AddAsync(new Notification
        {
            UserId = userId,
            RideId = reviewCreateDto.RideId,
            Title = "New review",
            Message = $"{_currentUserService.Username} reviewed you for the ride",
            Type = NotificationType.NewReview,
            SentAt = DateTime.UtcNow,
            IsRead = false
        });
        
        await _context.SaveChangesAsync();
        
        return Result<ReviewGetDto>.Success(new ReviewGetDto(
                new UserSummaryDto(
                    user.Id,
                    user.UserName,
                    user.ImageUrl,
                    user.Karma
                ),
            reviewCreateDto.RideId,
            reviewCreateDto.Karma,
            reviewCreateDto.Text,
            DateTime.UtcNow
            )
        );
    }
}