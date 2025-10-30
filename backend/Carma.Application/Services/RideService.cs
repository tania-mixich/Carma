using Carma.Application.Abstractions;
using Carma.Application.Abstractions.Repositories;
using Carma.Application.Common;
using Carma.Application.DTOs.Ride;
using Carma.Application.Mappers;
using Carma.Domain.Entities;
using Carma.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Carma.Application.Services;

public class RideService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRideRepository _rideRepository;
    private readonly UserManager<User> _userManager;
    private readonly IValidator<RideCreateDto> _createValidator;
    private readonly IValidator<RideUpdateDto> _updateValidator;

    public RideService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IRideRepository rideRepository, IValidator<RideCreateDto> createValidator, IValidator<RideUpdateDto> updateValidator, UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _rideRepository = rideRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _userManager = userManager;
    }

    public async Task<Result<IEnumerable<RideGetDto>>> GetAllRidesAsync()
    {
        var rides = await _rideRepository.GetAllAsync();
        return Result<IEnumerable<RideGetDto>>.Success(rides.Select(RideMapper.MapToRideGetDto));
    }

    public async Task<Result<IEnumerable<RideGetDto>>> GetNearbyRidesAsync(int radius = 1000)
    {
        var userId = _currentUserService.UserId;
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result<IEnumerable<RideGetDto>>.Failure("User not found");
        }
        var rides = await _rideRepository.GetNearbyRidesAsync(user.Location.Coordinate, radius);
        return Result<IEnumerable<RideGetDto>>.Success(rides.Select(RideMapper.MapToRideGetDto));
    }
    
    public async Task<Result<RideDetailsDto>> GetRideAsync(int rideId)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);
        if (ride == null)
        {
            return Result<RideDetailsDto>.Failure("Ride not found");
        }
        
        return Result<RideDetailsDto>.Success(RideMapper.MapToRideDetailsDto(ride));
    }

    public async Task<Result<RideGetDto>> CreateRideAsync(RideCreateDto rideCreateDto)
    {
        var validationResult = await _createValidator.ValidateAsync(rideCreateDto);
        if (!validationResult.IsValid)
        {
            return Result<RideGetDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).First());       
        }
        
        var userId = _currentUserService.UserId;
        
        var ride = RideMapper.MapToRide(rideCreateDto);
        ride.OrganizerId = userId;
        ride.Organizer = await _userManager.FindByIdAsync(userId.ToString());
        
        await _rideRepository.AddAsync(ride);
        
        var rideParticipant = new RideParticipant
        {
            RequestedAt = DateTime.UtcNow,
            AcceptedAt = DateTime.UtcNow,
            IsAccepted = true,
            Ride = ride,
            UserId = userId,
            RideRole = RideRole.Organizer
        };
        
        await _unitOfWork.RideParticipants.AddAsync(rideParticipant);
        await _unitOfWork.SaveChangesAsync();
        
        return Result<RideGetDto>.Success(RideMapper.MapToRideGetDto(ride));
    }

    public async Task<Result<RideGetDto>> UpdateRideAsync(int id, RideUpdateDto rideUpdateDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(rideUpdateDto);
        if (!validationResult.IsValid)
        {
            return Result<RideGetDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).First());      
        }
        
        var ride = await _rideRepository.GetByIdAsync(id);
        if (ride == null)
        {
            return Result<RideGetDto>.Failure("Ride not found");
        }

        if (rideUpdateDto.PickupLocation != null)
        {
            ride.PickupLocation = LocationMapper.MapToLocation(rideUpdateDto.PickupLocation);   
        }

        if (rideUpdateDto.DropOffLocation != null)
        {
            ride.DropOffLocation = LocationMapper.MapToLocation(rideUpdateDto.DropOffLocation);
        }

        ride.PickupTime = rideUpdateDto.PickupTime ?? ride.PickupTime;
        ride.Price = rideUpdateDto.Price ?? ride.Price;
        ride.AvailableSeats = rideUpdateDto.AvailableSeats ?? ride.AvailableSeats;
        ride.UpdatedAt = DateTime.UtcNow;
        
        _rideRepository.Update(ride);
        await _unitOfWork.SaveChangesAsync();
        
        return Result<RideGetDto>.Success(RideMapper.MapToRideGetDto(ride));
    }

    public async Task<Result<RideGetDto>> UpdateRideStatusAsync(int rideId, Status status)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);
        if (ride == null)
        {
            return Result<RideGetDto>.Failure("Ride not found");
        }
        
        if (ride.OrganizerId != _currentUserService.UserId)
        {
            return Result<RideGetDto>.Failure("You are not the organizer of this ride");
        }
        
        ride.Status = status;
        ride.UpdatedAt = DateTime.UtcNow;

        if (status == Status.Completed)
        {
            var participants = ride.Participants
                .Where(rp => rp.IsAccepted)
                .Select(rp => rp.User)
                .ToList();

            foreach (var participant in participants)
            {
                participant.RidesCount++;
            }
            
            var notifications = ride.Participants
                .Where(rp => rp.UserId != _currentUserService.UserId && rp.IsAccepted)
                .Select(rp => new Notification
                {
                    UserId = rp.UserId,
                    RideId = rideId,
                    Title = "Ride completed",
                    Message = $"Ride organized by {_currentUserService.Username} completed",
                    Type = NotificationType.RideCompleted,
                    SentAt = DateTime.UtcNow,
                    IsRead = false
                });
            
            await _unitOfWork.Notifications.AddRangeAsync(notifications);
        }
        else if (status == Status.Cancelled)
        {
            var notifications = ride.Participants
                .Where(rp => rp.UserId != _currentUserService.UserId && rp.IsAccepted)
                .Select(rp => new Notification
                    {
                        UserId = rp.UserId,
                        RideId = rideId,
                        Title = "Ride cancelled",
                        Message = $"{_currentUserService.Username} cancelled the ride",
                        Type = NotificationType.RideCancelled,
                        SentAt = DateTime.UtcNow,
                    }
                );

            await _unitOfWork.Notifications.AddRangeAsync(notifications);
        }
        else if (status == Status.InProgress)
        {
            var notifications = ride.Participants
                .Where(rp => rp.UserId != _currentUserService.UserId && rp.IsAccepted)
                .Select(rp => new Notification
                {
                    UserId = rp.UserId,
                    RideId = rideId,
                    Title = "Ride in progress",
                    Message = $"{_currentUserService.Username} marked the ride as in progress",
                    Type = NotificationType.RideStarted,
                    SentAt = DateTime.UtcNow
                });
            
            await _unitOfWork.Notifications.AddRangeAsync(notifications);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result<RideGetDto>.Success(RideMapper.MapToRideGetDto(ride));
    }
}