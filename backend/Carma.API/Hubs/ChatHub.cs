using Carma.Application.DTOs.Message;
using Carma.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Carma.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly MessageService _messageService;
    private readonly RideParticipantService _rideParticipantService;
    
    public ChatHub(MessageService messageService, RideParticipantService rideParticipantService)
    {
        _messageService = messageService;
        _rideParticipantService = rideParticipantService;
    }
    
    public async Task JoinRide(int rideId)
    {
        var result = await _rideParticipantService.CanJoinChatAsync(rideId);
        if (!result.IsSuccess)
        {
            await Clients.Caller.SendAsync("Error", result.Error);
            return;
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, rideId.ToString());
        await Clients.Caller.SendAsync("JoinRide", rideId);
    }

    public async Task SendMessage(int rideId, string content)
    {
        var result = await _messageService.SendMessageAsync(rideId, new MessageCreateDto(content));
        
        if (!result.IsSuccess)
        {
            await Clients.Caller.SendAsync("Error", result.Error);
            return;
        }
        
        var message = result.Value;
        await Clients.Group(rideId.ToString()).SendAsync("ReceiveMessage", message.UserName, message.Text, message.SentAt);
    }
}