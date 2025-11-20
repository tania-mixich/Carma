using Carma.Application.Abstractions;
using Carma.Application.Common;
using Carma.Application.DTOs.RideParticipant;
using Carma.Domain.Entities;
using Carma.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Carma.Application.Services;

public class RideParticipantService
{
    private readonly ICarmaDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RideParticipantService(ICarmaDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<RideParticipantGetDto>> RequestToJoinRideAsync(int rideId)
    {
        var userId = _currentUserService.UserId;
        
        var ride = await _context.Rides.FindAsync(rideId);
        if (ride == null)
        {
            return Result<RideParticipantGetDto>.NotFound("Ride not found");
        }
        
        if (ride.OrganizerId == userId)
        {
            return Result<RideParticipantGetDto>.Conflict("You cannot request to join your own ride");
        }

        var participantCount = await _context.RideParticipants.CountAsync(rp => rp.RideId == rideId && rp.Status == ParticipantStatus.Accepted);
        
        if (ride.Seats - participantCount < 1)
        {
            return Result<RideParticipantGetDto>.Conflict("Ride is full");
        }
        
        var requestedParticipant =
            await _context.RideParticipants.FirstOrDefaultAsync(rp => rp.RideId == rideId && rp.UserId == userId);
        
        if (requestedParticipant != null)
        {
            if (requestedParticipant.Status == ParticipantStatus.Left)
            {
                requestedParticipant.Status = ParticipantStatus.Pending;
                requestedParticipant.RequestedAt = DateTime.UtcNow;
                requestedParticipant.LeftAt = null;
                _context.RideParticipants.Update(requestedParticipant);
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
                RideId = rideId,
                UserId = userId,
                RequestedAt = DateTime.UtcNow,
                Role = ParticipantRole.Passenger,
                Status = ParticipantStatus.Pending
            };

            await _context.RideParticipants.AddAsync(requestedParticipant);
        }

        await _context.Notifications.AddAsync(new Notification
        {
            UserId = ride.OrganizerId,
            RideId = rideId,
            Title = "Ride request",
            Message = $"{_currentUserService.Username} requested to join your ride",
            Type = NotificationType.JoinRequest,
            SentAt = DateTime.UtcNow,
            IsRead = false
        });
        await _context.SaveChangesAsync();

        var user = await _context.Users.FindAsync(userId);
        
        return Result<RideParticipantGetDto>.Success(
            new RideParticipantGetDto(
                user!.UserName,
                user.ImageUrl,
                user.Karma,
                requestedParticipant.Role.ToString()
                )
            );
    }
    
    public async Task<Result<RideParticipantGetDto?>> HandleRideParticipantAsync(int rideId, Guid rideParticipantId, RideParticipantUpdateDto updateDto)
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
        
        var userId = _currentUserService.UserId;
        if (userId == rideParticipantId)
        {
            return Result<RideParticipantGetDto?>.Conflict("You cannot accept your own ride request");
        }
        
        var ride = await _context.Rides.FindAsync(rideId);
        if (ride == null)
        {
            return Result<RideParticipantGetDto?>.NotFound("Ride not found");
        }

        if (ride.OrganizerId != userId)
        {
            return Result<RideParticipantGetDto?>.Unauthorized("You are not the organizer of this ride");
        }

        var rideParticipant = await _context.RideParticipants
            .Include(rp => rp.User)
            .FirstOrDefaultAsync(rp => rp.RideId == rideId && rp.UserId == rideParticipantId);
        
        if (rideParticipant == null)
        {
            return Result<RideParticipantGetDto?>.NotFound("Ride participant not found");
        }
        
        if (rideParticipant.Status != ParticipantStatus.Pending)
        {
            return Result<RideParticipantGetDto?>.Conflict("Ride participant already handled");
        }
        
        if (newStatus == ParticipantStatus.Accepted)
        {
            rideParticipant.AcceptedAt = DateTime.UtcNow;
            rideParticipant.Status = ParticipantStatus.Accepted;
            rideParticipant.Role = ParticipantRole.Passenger;
 
            var acceptedCount = await _context.RideParticipants.CountAsync(rp => rp.RideId == rideId && rp.Status == ParticipantStatus.Accepted);
            if (acceptedCount + 1 >= ride.Seats)
            {
                ride.Status = Status.Full;
            }
        
            _context.Rides.Update(ride);
            _context.RideParticipants.Update(rideParticipant);
        
            await _context.Notifications.AddAsync(new Notification
            {
                UserId = rideParticipantId,
                RideId = rideId,
                Title = "Ride request accepted",
                Message = $"{_currentUserService.Username} accepted your ride request",
                Type = NotificationType.JoinAccepted,
                SentAt = DateTime.UtcNow,
                IsRead = false
            });
            await _context.SaveChangesAsync();
            
            return Result<RideParticipantGetDto?>.Success(new RideParticipantGetDto(
                    rideParticipant.User.UserName,
                    rideParticipant.User.ImageUrl,
                    rideParticipant.User.Karma,
                    rideParticipant.Role.ToString()
                )
            );
        }
        rideParticipant.Status = ParticipantStatus.Rejected;
        rideParticipant.RejectedAt = DateTime.UtcNow;
        _context.RideParticipants.Update(rideParticipant);
    
        await _context.Notifications.AddAsync(new Notification
        {
            UserId = rideParticipantId,
            RideId = rideId,
            Title = "Ride request rejected",
            Message = $"{_currentUserService.Username} rejected your ride request",
            Type = NotificationType.JoinRejected,
            SentAt = DateTime.UtcNow,
            IsRead = false
        });
        await _context.SaveChangesAsync();
        
        return Result<RideParticipantGetDto?>.Success(null);
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
            
            _context.Rides.Update(ride);
        }

        rideParticipant.Status = ParticipantStatus.Left;
        rideParticipant.LeftAt = DateTime.UtcNow;
        _context.RideParticipants.Update(rideParticipant);
        
        await _context.Notifications.AddAsync(new Notification
        {
            UserId = ride.OrganizerId,
            RideId = rideId,
            Title = "Member left",
            Message = $"{rideParticipant.User.UserName} left the ride",
            Type = NotificationType.LeftRide,
            SentAt = DateTime.UtcNow,
            IsRead = false
        });
        
        await _context.SaveChangesAsync();
        
        return Result.Success();
    }
    
    public async Task<Result> CanJoinChatAsync(int rideId)
    {
        var userId = _currentUserService.UserId;
        var participant = await _context.RideParticipants
            .FirstOrDefaultAsync(rp => rp.RideId == rideId && rp.UserId == userId);
        if (participant == null)
            return Result.Unauthorized("You are not a participant of this ride");
    
        if (participant.Status != ParticipantStatus.Accepted)
            return Result.Conflict("Your request to join this ride is still pending");

        return Result.Success();
    }

}