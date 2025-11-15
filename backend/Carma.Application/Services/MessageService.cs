using Carma.Application.Abstractions;
using Carma.Application.Common;
using Carma.Application.DTOs.Message;
using Carma.Domain.Entities;
using Carma.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Carma.Application.Services;

public class MessageService
{
    private readonly ICarmaDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    
    public MessageService(ICarmaDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ICollection<MessageGetDto>>> GetMessagesFromRideAsync(int rideId)
    {
        var rideExists = await _context.Rides.AnyAsync(r => r.Id == rideId);
        if (!rideExists)
        {
            return Result<ICollection<MessageGetDto>>.NotFound("Ride not found");
        }
        
        var userId = _currentUserService.UserId;
        
        var isParticipant = await _context.RideParticipants
            .AnyAsync(rp => rp.UserId == userId && rp.RideId == rideId && rp.Status == ParticipantStatus.Accepted);

        if (!isParticipant)
        {
            return Result<ICollection<MessageGetDto>>.Unauthorized("You are not part of this ride");
        }
        
        var messages = await _context.Messages
            .Where(m => m.RideId == rideId)
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageGetDto(
                m.User.UserName, 
                m.Text,
                m.SentAt
                )
            )
            .ToListAsync();
        
        return Result<ICollection<MessageGetDto>>.Success(messages);
    }

    public async Task<Result<MessageGetDto>> SendMessageAsync(int rideId, MessageCreateDto messageCreateDto)
    {
        var rideExists = await _context.Rides.AnyAsync(r => r.Id == rideId);
        if (!rideExists)
        {
            return Result<MessageGetDto>.NotFound("Ride not found");
        }
        
        var userId = _currentUserService.UserId;
        
        var isParticipant = await _context.RideParticipants.AnyAsync(rp => rp.UserId == userId && rp.RideId == rideId && rp.Status == ParticipantStatus.Accepted);
        if (!isParticipant)
        {
            return Result<MessageGetDto>.Unauthorized("You cannot send messages on rides that you are not in");
        }

        var message = new Message
        {
            Text = messageCreateDto.Message,
            RideId = rideId,
            UserId = userId,
            SentAt = DateTime.UtcNow
        };
        
        await _context.Messages.AddAsync(message);
        
        var otherParticipants = await _context.RideParticipants
            .Where(rp => rp.RideId == rideId && rp.UserId != userId && rp.Status == ParticipantStatus.Accepted)
            .Select(rp => rp.UserId)
            .ToListAsync();

        var notifications = otherParticipants.Select(participantId => new Notification
        {
            UserId = participantId,
            RideId = rideId,
            Title = "New message in ride",
            Message = $"{_currentUserService.Username} sent a message",
            Type = NotificationType.NewMessage,
            SentAt = DateTime.UtcNow,
            IsRead = false
        });
        
        await _context.Notifications.AddRangeAsync(notifications);
        await _context.SaveChangesAsync();
        
        return Result<MessageGetDto>.Success(new MessageGetDto(
            _currentUserService.Username,
            message.Text,
            message.SentAt
            )
        );
    }
}