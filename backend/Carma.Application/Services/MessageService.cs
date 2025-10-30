using Carma.Application.Abstractions;
using Carma.Application.Abstractions.Repositories;
using Carma.Application.Common;
using Carma.Application.DTOs.Message;
using Carma.Application.Mappers;
using Carma.Domain.Entities;
using Carma.Domain.Enums;

namespace Carma.Application.Services;

public class MessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IRideRepository _rideRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    
    public MessageService(IMessageRepository messageRepository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IRideRepository rideRepository)
    {
        _messageRepository = messageRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _rideRepository = rideRepository;
    }

    public async Task<Result<ICollection<MessageGetDto>>> GetMessagesFromRideAsync(int rideId)
    {
        var messages = await _messageRepository.GetAllFromRideAsync(rideId);
        messages = messages.OrderBy(m => m.SentAt);
        return Result<ICollection<MessageGetDto>>.Success(messages.Select(MessageMapper.MapToMessageGetDto).ToList());
    }

    public async Task<Result<MessageGetDto>> SendMessageAsync(int rideId, MessageCreateDto messageCreateDto)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);
        if (ride == null)
        {
            return Result<MessageGetDto>.Failure("Ride not found");
        }
        
        var userId = _currentUserService.UserId;
        
        var isParticipant = ride.Participants.Any(rp => rp.UserId == userId && rp.IsAccepted);
        if (!isParticipant)
        {
            return Result<MessageGetDto>.Failure("You are not a participant of this ride");
        }

        var message = new Message
        {
            Text = messageCreateDto.Message,
            RideId = rideId,
            UserId = userId,
            SentAt = DateTime.UtcNow
        };
        await _messageRepository.AddAsync(message);
        
        var otherParticipants = ride.Participants
            .Where(rp => rp.UserId != userId && rp.IsAccepted)
            .Select(rp => new Notification
            {
                UserId = rp.UserId,
                RideId = rideId,
                Title = "New message in ride",
                Message = $"{_currentUserService.Username} sent a message",
                Type = NotificationType.NewMessage,
                SentAt = DateTime.UtcNow,
                IsRead = false
            });
        
        await _unitOfWork.Notifications.AddRangeAsync(otherParticipants);
        await _unitOfWork.SaveChangesAsync();
        
        return Result<MessageGetDto>.Success(MessageMapper.MapToMessageGetDto(message));
    }
}