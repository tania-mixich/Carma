using Carma.Application.DTOs.Message;
using Carma.Application.DTOs.Notification;

namespace Carma.Application.Abstractions;

public interface IRealTimeNotifier
{
    Task ChatMessageReceivedAsync(int rideId, MessageGetDto message);
    Task NotificationReceivedAsync(Guid userId, NotificationGetDto notification);
    Task RideStatusUpdatedAsync(int rideId, string newStatus);
}