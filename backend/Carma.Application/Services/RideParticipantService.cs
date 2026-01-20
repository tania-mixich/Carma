using Carma.Application.Abstractions;
using Carma.Application.Common;
using Carma.Application.DTOs.Notification;
using Carma.Application.DTOs.RideParticipant;
using Carma.Domain.Entities;
using Carma.Domain.Enums;
using Carma.Domain.Factories;
using Microsoft.EntityFrameworkCore;

namespace Carma.Application.Services;

public class RideParticipantService
{
    private readonly ICarmaDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRealTimeNotifier _realTimeNotifier;

    public RideParticipantService(ICarmaDbContext context, ICurrentUserService currentUserService, IRealTimeNotifier realTimeNotifier)
    {
        _context = context;
        _currentUserService = currentUserService;
        _realTimeNotifier = realTimeNotifier;
    }

    public async Task<Result<List<RideParticipantGetDto>>> GetPendingParticipantsAsync(int rideId)
    {
        var userId = _currentUserService.UserId;

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return Result<List<RideParticipantGetDto>>.NotFound("User not found");
        }

        var ride = await _context.Rides.FindAsync(rideId);
        if (ride == null)
        {
            return Result<List<RideParticipantGetDto>>.NotFound("Ride not found");
        }

        if (ride.OrganizerId != userId)
        {
            return Result<List<RideParticipantGetDto>>.Unauthorized("You are not the organizer of the ride");
        }

        var pendingParticipants = await _context.RideParticipants
            .Where(rp => rp.RideId == rideId && rp.Status == ParticipantStatus.Pending)
            .Select(rp => new RideParticipantGetDto(
                rp.User.Id,
                rp.User.UserName ?? string.Empty,
                rp.User.ImageUrl ?? string.Empty,
                rp.User.Karma,
                rp.Role.ToString()
                ))
            .ToListAsync();
        
        return Result<List<RideParticipantGetDto>>.Success(pendingParticipants);

    }

    public async Task<Result<RideParticipantGetDto>> RequestToJoinRideAsync(int rideId)
    {
        var userId = _currentUserService.UserId;
        
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return Result<RideParticipantGetDto>.NotFound("User not found");
        }
        
        var ride = await _context.Rides
            .Include(r => r.Participants)
            .FirstOrDefaultAsync(r => r.Id == rideId);
        if (ride == null)
        {
            return Result<RideParticipantGetDto>.NotFound("Ride not found");
        }
        
        if (ride.OrganizerId == userId)
        {
            return Result<RideParticipantGetDto>.Conflict("You cannot request to join your own ride");
        }

        var participantCount = ride.Participants.Count(rp => rp.Status == ParticipantStatus.Accepted);
        
        if (ride.Seats - participantCount < 1)
        {
            return Result<RideParticipantGetDto>.Conflict("Ride is full");
        }
        
        var requestedParticipant = ride.Participants.FirstOrDefault(rp => rp.UserId == userId);
        
        if (requestedParticipant != null)
        {
            if (requestedParticipant.Status == ParticipantStatus.Left)
            {
                requestedParticipant.Status = ParticipantStatus.Pending;
                requestedParticipant.RequestedAt = DateTime.UtcNow;
                requestedParticipant.LeftAt = null;
            }
            else {
                return Result<RideParticipantGetDto>.Conflict(
                    "You already requested to join this ride or you've been rejected");
            }
        }
        else
        {
            requestedParticipant = new RideParticipant
            {
                UserId = userId,
                RequestedAt = DateTime.UtcNow,
                Role = ParticipantRole.Passenger,
                Status = ParticipantStatus.Pending
            };

            ride.Participants.Add(requestedParticipant);
        }

        var notification = NotificationFactory.CreateJoinRequest(ride.OrganizerId, rideId, _currentUserService.Username);
        
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();

        var notificationDto = new NotificationGetDto(
            notification.Id,
            notification.Title,
            notification.Message,
            notification.Type.ToString(),
            notification.SentAt,
            notification.RideId,
            false
        );
        
        await _realTimeNotifier.NotificationReceivedAsync(notification.UserId, notificationDto);
        
        return Result<RideParticipantGetDto>.Success(
            new RideParticipantGetDto(
                user.Id,
                user.UserName ?? string.Empty,
                user.ImageUrl,
                user.Karma,
                requestedParticipant.Role.ToString()
                )
            );
    }
    
    public async Task<Result<RideParticipantGetDto?>> HandleRideParticipantAsync(int rideId, Guid targetUserId, RideParticipantUpdateDto updateDto)
    {
        var statusString = updateDto.Status;
        if (!Enum.TryParse<ParticipantStatus>(statusString, true, out var newStatus))
        {
            return Result<RideParticipantGetDto?>.Conflict("Invalid status provided");
        }
        
        if (newStatus != ParticipantStatus.Accepted && newStatus != ParticipantStatus.Rejected)
        {
            return Result<RideParticipantGetDto?>.Conflict("You can only Accept or Reject");
        }
        
        if (_currentUserService.UserId == targetUserId)
        {
            return Result<RideParticipantGetDto?>.Conflict("You cannot accept your own ride request");
        }
        
        var ride = await _context.Rides
            .Include(r => r.Participants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(r => r.Id == rideId);
        if (ride == null)
        {
            return Result<RideParticipantGetDto?>.NotFound("Ride not found");
        }

        if (ride.OrganizerId != _currentUserService.UserId)
        {
            return Result<RideParticipantGetDto?>.Unauthorized("You are not the organizer of this ride");
        }

        var participant = ride.Participants.FirstOrDefault(p => p.UserId == targetUserId);
        
        if (participant == null)
        {
            return Result<RideParticipantGetDto?>.NotFound("Ride participant not found");
        }
        
        if (participant.Status != ParticipantStatus.Pending)
        {
            return Result<RideParticipantGetDto?>.Conflict("Ride participant already handled");
        }

        Notification notification;
        
        if (newStatus == ParticipantStatus.Accepted)
        {
            participant.AcceptedAt = DateTime.UtcNow;
            participant.Status = ParticipantStatus.Accepted;
            participant.Role = ParticipantRole.Passenger;

            var acceptedCount =  ride.Participants.Count(rp => rp.RideId == rideId && rp.Status == ParticipantStatus.Accepted);
            
            if (acceptedCount >= ride.Seats)
            {
                ride.Status = Status.Full;
            }

            notification = NotificationFactory.CreateJoinAccepted(targetUserId, rideId, _currentUserService.Username);
        }
        else
        {
            participant.Status = ParticipantStatus.Rejected;
            participant.RejectedAt = DateTime.UtcNow;

            notification = NotificationFactory.CreateJoinRejected(targetUserId, rideId, _currentUserService.Username);
        }

        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();

        var notificationDto = new NotificationGetDto(
            notification.Id,
            notification.Title,
            notification.Message,
            notification.Type.ToString(),
            notification.SentAt,
            notification.RideId,
            false
        );

        await _realTimeNotifier.NotificationReceivedAsync(notification.UserId, notificationDto);

        if (newStatus == ParticipantStatus.Rejected)
        {
            return Result<RideParticipantGetDto?>.Success(null);
        }

        return Result<RideParticipantGetDto?>.Success(new RideParticipantGetDto(
            participant.User.Id,
            participant.User.UserName ?? string.Empty, 
            participant.User.ImageUrl, 
            participant.User.Karma, 
            participant.Role.ToString())
        );
    }
    
    public async Task<Result> LeaveRideAsync(int rideId)
    {
        var userId = _currentUserService.UserId;
        
        var ride = await _context.Rides.FindAsync(rideId);
        if (ride == null)
        {
            return Result.NotFound("Ride not found");
        }

        var rideParticipant = await _context.RideParticipants
            .Include(rp => rp.User)
            .FirstOrDefaultAsync(rp => rp.RideId == rideId && rp.UserId == userId);
        if (rideParticipant == null)
        {
            return Result.Unauthorized("You are not a participant of this ride");
        }

        if (rideParticipant.Role == ParticipantRole.Organizer)
        {
            return Result.Conflict("You cannot leave your own ride");
        }

        if (rideParticipant.Status == ParticipantStatus.Accepted)
        {
            if (ride.Status == Status.Full)
            {
                ride.Status = Status.Available;       
            }
        }

        rideParticipant.Status = ParticipantStatus.Left;
        rideParticipant.LeftAt = DateTime.UtcNow;

        var notification = NotificationFactory.CreateLeftRide(ride.OrganizerId, rideId, rideParticipant.User.UserName ?? string.Empty);
        
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
        
        var notificationDto = new NotificationGetDto(
            notification.Id,
            notification.Title,
            notification.Message,
            notification.Type.ToString(),
            notification.SentAt,
            notification.RideId,
            false
        );
        
        await _realTimeNotifier.NotificationReceivedAsync(notification.UserId, notificationDto);
        
        return Result.Success();
    }
}