using Carma.Application.Abstractions;
using Carma.Application.Abstractions.Repositories;
using Carma.Application.Common;
using Carma.Application.DTOs.Review;
using Carma.Application.Mappers;
using Carma.Domain.Entities;
using Carma.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Carma.Application.Services;

public class ReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ReviewCreateDto> _createValidator;

    public ReviewService(IReviewRepository reviewRepository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, UserManager<User> userManager, IValidator<ReviewCreateDto> createValidator)
    {
        _reviewRepository = reviewRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _createValidator = createValidator;
    }

    public async Task<Result<IEnumerable<ReviewGetDto>>> GetAllForUserAsync(Guid userId)
    {
        var reviews = await _reviewRepository.GetAllForUserAsync(userId);
        return Result<IEnumerable<ReviewGetDto>>.Success(reviews.Select(ReviewMapper.MapToGetDto));
    }

    public async Task<Result<ReviewGetDto>> AddReviewAsync(Guid userId, ReviewCreateDto reviewCreateDto)
    {
        var validationResult = await _createValidator.ValidateAsync(reviewCreateDto);
        if (!validationResult.IsValid)
        {
            return Result<ReviewGetDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).First());
        }
        
        var currentUserId = _currentUserService.UserId;
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result<ReviewGetDto>.Failure("User not found");
        }

        if (user.Id == currentUserId)
        {
            return Result<ReviewGetDto>.Failure("You cannot review yourself");
        }
        
        var ride = await _unitOfWork.Rides.GetByIdAsync(reviewCreateDto.RideId);
        if (ride == null)
        {
            return Result<ReviewGetDto>.Failure("Ride not found");
        }

        if (ride.Status != Status.Completed)
        {
            return Result<ReviewGetDto>.Failure("You cannot review a ride that is not completed");
        }

        var isParticipantOfRide =
            await _unitOfWork.RideParticipants.ContainsUserAsync(reviewCreateDto.RideId,
                userId);
        if (!isParticipantOfRide)
        {
            return Result<ReviewGetDto>.Failure("User is not a participant of this ride");
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
        
        await _reviewRepository.AddAsync(review);
        
        user.Karma = (user.Karma * user.ReviewsCount + reviewCreateDto.Karma) / (user.ReviewsCount + 1);
        user.ReviewsCount++;

        await _unitOfWork.Notifications.CreateAsync(new Notification
        {
            UserId = userId,
            RideId = reviewCreateDto.RideId,
            Title = "New review",
            Message = $"{_currentUserService.Username} reviewed you for the ride",
            Type = NotificationType.NewReview,
            SentAt = DateTime.UtcNow,
            IsRead = false
        });
        
        await _unitOfWork.SaveChangesAsync();
        
        return Result<ReviewGetDto>.Success(ReviewMapper.MapToGetDto(review));
    }
}