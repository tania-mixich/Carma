using Carma.Application.Abstractions;
using Carma.Application.Common;
using Carma.Application.DTOs.Notification;
using Microsoft.EntityFrameworkCore;

namespace Carma.Application.Services;

public class NotificationService
{
    private readonly ICarmaDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    
    public NotificationService(ICarmaDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IEnumerable<NotificationGetDto>>> GetAllForUserAsync()
    {
        var userId = _currentUserService.UserId;
        
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.SentAt)
            .Select(n => new NotificationGetDto(
                n.Id,
                n.Title,
                n.Message,
                n.Type.ToString(),
                n.SentAt,
                n.RideId,
                n.IsRead
                )
            )
            .ToListAsync();
        
        return Result<IEnumerable<NotificationGetDto>>.Success(notifications);
    }

    public async Task<Result<IEnumerable<NotificationGetDto>>> GetAllUnreadForUserAsync()
    {
        var userId = _currentUserService.UserId;
        
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && n.IsRead == false)
            .OrderByDescending(n => n.SentAt)
            .Select(n => new NotificationGetDto(
                n.Id,
                n.Title,
                n.Message,
                n.Type.ToString(),
                n.SentAt,
                n.RideId,
                n.IsRead
                )
            )
            .ToListAsync();
        
        return Result<IEnumerable<NotificationGetDto>>.Success(notifications);
    }
    public async Task<Result<NotificationGetDto>> MarkAsReadAsync(int notificationId, NotificationUpdateDto requestDto)
    {
        if (requestDto.IsRead != true)
        {
            return Result<NotificationGetDto>.Conflict("You can only mark it as read");
        }
        
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification == null)
        {
            return Result<NotificationGetDto>.NotFound("Notification not found");
        }

        if (notification.IsRead)
        {
            return Result<NotificationGetDto>.Conflict("Notification already read");
        }

        if (notification.UserId != _currentUserService.UserId)
        {
            return Result<NotificationGetDto>.Unauthorized("You cannot mark this notification as read");
        }
        
        notification.IsRead = true;
        
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
        
        return Result<NotificationGetDto>.Success(new NotificationGetDto(
            notification.Id,
            notification.Title,
            notification.Message,
            notification.Type.ToString(),
            notification.SentAt,
            notification.RideId,
            notification.IsRead
            )
        );
    }
}