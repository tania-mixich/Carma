using Carma.Application.Abstractions;
using Carma.Application.Abstractions.Repositories;
using Carma.Application.Common;
using Carma.Application.DTOs.Ride;
using Carma.Application.Mappers;
using Carma.Domain.Entities;
using Carma.Domain.Enums;
using FluentValidation;

namespace Carma.Application.Services;

public class RideService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRideRepository _rideRepository;
    private readonly IValidator<RideCreateDto> _createValidator;
    private readonly IValidator<RideUpdateDto> _updateValidator;

    public RideService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IRideRepository rideRepository, IValidator<RideCreateDto> createValidator, IValidator<RideUpdateDto> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _rideRepository = rideRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<IEnumerable<RideGetDto>>> GetAllRidesAsync()
    {
        var rides = await _rideRepository.GetAllAsync();
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

        ride.PickupLocation = LocationMapper.MapToLocation(rideUpdateDto.PickupLocation);
        ride.DropOffLocation = LocationMapper.MapToLocation(rideUpdateDto.DropOffLocation);
        ride.PickupTime = rideUpdateDto.PickupTime;
        ride.Price = rideUpdateDto.Price;
        ride.AvailableSeats = rideUpdateDto.AvailableSeats;
        ride.UpdatedAt = DateTime.UtcNow;
        
        _rideRepository.Update(ride);
        await _unitOfWork.SaveChangesAsync();
        
        return Result<RideGetDto>.Success(RideMapper.MapToRideGetDto(ride));
    }
    
    public async Task<Result> DeleteRideAsync(int rideId)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);
        if (ride == null)
        {
            return Result.Failure("Ride not found");
        }
        
        _rideRepository.Delete(ride);
        
        var participants = ride.Participants
            .Where(rp => rp.UserId != _currentUserService.UserId && rp.IsAccepted)
            .Select(rp => new Notification
            {
                UserId = rp.UserId,
                RideId = rideId,
                Title = "Ride cancelled",
                Message = $"{_currentUserService.Email} cancelled the ride",
                Type = NotificationType.RideCancelled,
                SentAt = DateTime.UtcNow,
                IsRead = false
            });

        await _unitOfWork.Notifications.AddRangeAsync(participants);
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success();
    }
}