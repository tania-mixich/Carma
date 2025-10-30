using Carma.Application.Abstractions;
using Carma.Application.Abstractions.Repositories;
using Carma.Application.Common;
using Carma.Application.DTOs.Notification;
using Carma.Application.Mappers;
using Carma.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Carma.Application.Services;

public class NotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    
    public NotificationService(INotificationRepository notificationRepository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<NotificationGetDto>>> GetAllForUserAsync()
    {
        var userId = _currentUserService.UserId;
        var notifications = await _notificationRepository.GetAllForUserAsync(userId);
        return Result<IEnumerable<NotificationGetDto>>.Success(notifications.Select(NotificationMapper.MapToNotificationGetDto));
    }

    public async Task<Result<IEnumerable<NotificationGetDto>>> GetAllUnreadForUserAsync()
    {
        var userId = _currentUserService.UserId;
        var notifications = await _notificationRepository.GetUnreadForUserAsync(userId);
        return Result<IEnumerable<NotificationGetDto>>.Success(notifications.Select(NotificationMapper.MapToNotificationGetDto));
    }
    public async Task<Result<NotificationGetDto>> MarkAsReadAsync(int notificationId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification == null)
        {
            return Result<NotificationGetDto>.Failure("Notification not found");
        }

        if (notification.IsRead)
        {
            return Result<NotificationGetDto>.Failure("Notification already read");
        }

        if (notification.UserId != _currentUserService.UserId)
        {
            return Result<NotificationGetDto>.Failure("You cannot mark this notification as read");
        }
        
        notification.IsRead = true;
        _notificationRepository.Update(notification);
        await _unitOfWork.SaveChangesAsync();
        return Result<NotificationGetDto>.Success(NotificationMapper.MapToNotificationGetDto(notification));
    }
}