using Carma.Application.DTOs.Message;
using Carma.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Carma.API.Controllers;

[ApiController]
[Route("rides/{rideId}/messages")]
public class MessageController : ControllerBase
{
    private readonly MessageService _messageService;
    
    public MessageController(MessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages(int rideId)
    {
        var result = await _messageService.GetMessagesFromRideAsync(rideId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(int rideId, MessageCreateDto messageCreateDto)
    {
        var result = await _messageService.SendMessageAsync(rideId, messageCreateDto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}