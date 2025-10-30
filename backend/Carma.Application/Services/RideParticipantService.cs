using Carma.Application.Abstractions;
using Carma.Application.Abstractions.Repositories;
using Carma.Application.Common;
using Carma.Application.DTOs.RideParticipant;
using Carma.Application.Mappers;
using Carma.Domain.Entities;
using Carma.Domain.Enums;

namespace Carma.Application.Services;

public class RideParticipantService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRideParticipantRepository _rideParticipantRepository;
    private readonly IRideRepository _rideRepository;

    public RideParticipantService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
        IRideParticipantRepository rideParticipantRepository, IRideRepository rideRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _rideParticipantRepository = rideParticipantRepository;
        _rideRepository = rideRepository;
    }

    public async Task<Result<RideParticipantGetDto>> RequestToJoinRideAsync(int rideId)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);
        if (ride == null)
        {
            return Result<RideParticipantGetDto>.Failure("Ride not found");
        }
        
        if (ride.OrganizerId == _currentUserService.UserId)
        {
            return Result<RideParticipantGetDto>.Failure("You cannot request to join your own ride");
        }

        if (ride.AvailableSeats < 1)
        {
            return Result<RideParticipantGetDto>.Failure("Ride is full");
        }
        
        var containsResult = await _rideParticipantRepository.ContainsUserAsync(rideId, _currentUserService.UserId);
        if (containsResult)
        {
            return Result<RideParticipantGetDto>.Failure("You already requested to join this ride");
        }
        
        var rideParticipant = RideParticipantMapper.MapToRideParticipant(rideId);
        
        var userId = _currentUserService.UserId;
        rideParticipant.UserId = userId;
        rideParticipant.RequestedAt = DateTime.UtcNow;
        rideParticipant.RideRole = RideRole.NotAssigned;
        
        await _rideParticipantRepository.AddAsync(rideParticipant);
        await _unitOfWork.Notifications.CreateAsync(new Notification
        {
            UserId = ride.OrganizerId,
            RideId = rideId,
            Title = "Ride request",
            Message = $"{_currentUserService.Username} requested to join your ride",
            Type = NotificationType.JoinRequest,
            SentAt = DateTime.UtcNow,
            IsRead = false
        });
        await _unitOfWork.SaveChangesAsync();
        
        var created = await _rideParticipantRepository.GetByRideAndUserAsync(rideId, userId);
        return Result<RideParticipantGetDto>.Success(RideParticipantMapper.MapToRideParticipantGetDto(created!));
    }
    
    public async Task<Result<RideParticipantGetDto>> AcceptRideParticipantAsync(int rideId, Guid rideParticipantId)
    {
        if (_currentUserService.UserId == rideParticipantId)
        {
            return Result<RideParticipantGetDto>.Failure("You cannot accept your own ride request");
        }
        
        var ride = await _rideRepository.GetByIdAsync(rideId);
        if (ride == null)
        {
            return Result<RideParticipantGetDto>.Failure("Ride not found");
        }

        if (ride.OrganizerId != _currentUserService.UserId)
        {
            return Result<RideParticipantGetDto>.Failure("You are not the organizer of this ride");
        }

        var rideParticipant = await _rideParticipantRepository.GetByRideAndUserAsync(rideId, rideParticipantId);
        if (rideParticipant == null)
        {
            return Result<RideParticipantGetDto>.Failure("Ride participant not found");
        }
        
        if (rideParticipant.RideRole != RideRole.NotAssigned)
        {
            return Result<RideParticipantGetDto>.Failure("Ride participant already handled");
        }
        
        rideParticipant.AcceptedAt = DateTime.UtcNow;
        rideParticipant.IsAccepted = true;
        rideParticipant.RideRole = RideRole.Participant;
        ride.AvailableSeats--;
        var acceptedCount = ride.Participants.Count(p => p.IsAccepted);
        ride.PricePerSeat = ride.Price / acceptedCount;
        
        if (ride.AvailableSeats < 1)
        {
            ride.Status = Status.Full;
        }
        
        _rideRepository.Update(ride);
        _rideParticipantRepository.Update(rideParticipant);
        
        await _unitOfWork.Notifications.CreateAsync(new Notification
        {
            UserId = rideParticipantId,
            RideId = rideId,
            Title = "Ride request accepted",
            Message = $"{_currentUserService.Username} accepted your ride request",
            Type = NotificationType.JoinAccepted,
            SentAt = DateTime.UtcNow,
            IsRead = false
        });
        await _unitOfWork.SaveChangesAsync();
        
        return Result<RideParticipantGetDto>.Success(RideParticipantMapper.MapToRideParticipantGetDto(rideParticipant));
    }
    
    public async Task<Result> RejectRideParticipantAsync(int rideId, Guid rideParticipantId)
    {
        if (_currentUserService.UserId == rideParticipantId)
        {
            return Result.Failure("You cannot reject your own ride request");
        }
        
        var ride = await _rideRepository.GetByIdAsync(rideId);
        if (ride == null)
        {
            return Result.Failure("Ride not found");
        }
        
        if (ride.OrganizerId != _currentUserService.UserId)
            return Result.Failure("You are not the organizer of this ride");
        
        var rideParticipant = await _rideParticipantRepository.GetByRideAndUserAsync(rideId, rideParticipantId);
        if (rideParticipant == null)
        {
            return Result.Failure("Ride participant not found");
        }
        
        _rideParticipantRepository.Delete(rideParticipant);
        await _unitOfWork.Notifications.CreateAsync(new Notification
        {
            UserId = rideParticipantId,
            RideId = rideId,
            Title = "Ride request rejected",
            Message = $"{_currentUserService.Username} rejected your ride request",
            Type = NotificationType.JoinRejected,
            SentAt = DateTime.UtcNow,
            IsRead = false
        });
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success();
    }

    public async Task<Result> LeaveRideAsync(int rideId)
    {
        var userId = _currentUserService.UserId;
        
        var ride = await _rideRepository.GetByIdAsync(rideId);
        if (ride == null)
        {
            return Result.Failure("Ride not found");
        }

        var rideParticipant = await _rideParticipantRepository.GetByRideAndUserAsync(rideId, userId);
        if (rideParticipant == null)
        {
            return Result.Failure("You are not a participant of this ride");
        }

        if (rideParticipant.RideRole == RideRole.Organizer)
        {
            return Result.Failure("You cannot leave your own ride");
        }
        
        _rideParticipantRepository.Delete(rideParticipant);
        await _unitOfWork.Notifications.CreateAsync(new Notification
        {
            UserId = ride.OrganizerId,
            RideId = rideId,
            Title = "Member left",
            Message = $"{rideParticipant.User.UserName} left the ride",
            Type = NotificationType.LeftRide,
            SentAt = DateTime.UtcNow,
            IsRead = false
        });
        
        ride.AvailableSeats++;
        if (ride.Status == Status.Full)
        {
            ride.Status = Status.Available;       
        }
        
        var acceptedCount = ride.Participants.Count(p => p.IsAccepted);
        ride.PricePerSeat = ride.Price / (acceptedCount - 1);
        
        _rideRepository.Update(ride);
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success();
    }
}