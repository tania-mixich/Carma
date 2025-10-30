namespace Carma.Application.DTOs.Notification;

public record NotificationGetDto(
    int Id,
    string Title,
    string Message,
    string NotificationType,
    DateTime SentAt,
    int RideId
    );