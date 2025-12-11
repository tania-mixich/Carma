using Carma.Application.Abstractions;
using Carma.Application.Common;
using Carma.Application.DTOs.Message;
using Carma.Application.DTOs.Notification;
using Carma.Domain.Entities;
using Carma.Domain.Enums;
using Carma.Domain.Factories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Carma.Application.Services;

public class MessageService
{
    private readonly ICarmaDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<MessageCreateDto> _createValidator;
    private readonly IRealTimeNotifier _realTimeNotifier;
    
    public MessageService(ICarmaDbContext context, ICurrentUserService currentUserService, IRealTimeNotifier realTimeNotifier, IValidator<MessageCreateDto> createValidator)
    {
        _context = context;
        _currentUserService = currentUserService;
        _realTimeNotifier = realTimeNotifier;
        _createValidator = createValidator;
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
            .AsNoTracking()
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
        var validationResult = await _createValidator.ValidateAsync(messageCreateDto);
        if (!validationResult.IsValid)
        {
            return Result<MessageGetDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).First());
        }
        
        var ride = await _context.Rides
            .Include(r => r.Participants)
            .FirstOrDefaultAsync(r => r.Id == rideId);
        
        if (ride == null)
        {
            return Result<MessageGetDto>.NotFound("Ride not found");
        }
        
        var userId = _currentUserService.UserId;
        
        var isParticipant = ride.Participants
            .Any(rp => rp.UserId == userId && rp.Status == ParticipantStatus.Accepted);
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

        var notifications = ride.Participants
            .Where(rp => rp.UserId != userId && rp.Status == ParticipantStatus.Accepted)
            .Select(rp => rp.UserId)
            .Select(participantId =>
                NotificationFactory.CreateMessage(participantId, rideId, _currentUserService.Username))
            .ToList();
        
        if (notifications.Any())
        {
            await _context.Notifications.AddRangeAsync(notifications);
        }
        
        await _context.SaveChangesAsync();
        
        var messageDto = new MessageGetDto(
            _currentUserService.Username,
            message.Text,
            message.SentAt
        );

        await _realTimeNotifier.ChatMessageReceivedAsync(rideId, messageDto);

        foreach (var notification in notifications)
        {
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
        }

        return Result<MessageGetDto>.Success(messageDto);
    }
}