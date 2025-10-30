using Carma.Application.DTOs.Notification;
using Carma.Domain.Entities;

namespace Carma.Application.Mappers;

public static class NotificationMapper
{
    public static NotificationGetDto MapToNotificationGetDto(Notification notification)
    {
        return new NotificationGetDto(
            notification.Id,
            notification.Title,
            notification.Message,
            notification.Type.ToString(),
            notification.SentAt,
            notification.RideId
            );
    }
}