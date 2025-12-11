using Carma.API.Hubs;
using Carma.Application.Abstractions;
using Carma.Application.DTOs.Message;
using Carma.Application.DTOs.Notification;
using Microsoft.AspNetCore.SignalR;

namespace Carma.API.Services;

public class SignalRNotifierService : IRealTimeNotifier
{
    private readonly IHubContext<CarmaHub> _hubContext;

    public SignalRNotifierService(IHubContext<CarmaHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task ChatMessageReceivedAsync(int rideId, MessageGetDto message)
    {
        await _hubContext.Clients.Group($"Ride_{rideId.ToString()}").SendAsync("ReceiveMessage", message);
    }

    public async Task NotificationReceivedAsync(Guid userId, NotificationGetDto notification)
    {
        await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", notification);
    }

    public async Task RideStatusUpdatedAsync(int rideId, string newStatus)
    {
        await _hubContext.Clients.Group($"Ride_{rideId.ToString()}").SendAsync("RideStatusUpdated", newStatus);
    }
}