using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Carma.API.Hubs;

[Authorize]
public class CarmaHub : Hub
{
    public async Task JoinRideGroup(int rideId) 
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Ride_{rideId.ToString()}");
    }
    
    public async Task LeaveRideGroup(int rideId) 
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Ride_{rideId.ToString()}");
    }
}