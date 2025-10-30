using Carma.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Carma.API.Controllers;

[ApiController]
[Route("/notifications")]
public class NotificationController : ControllerBase
{
    private readonly NotificationService _notificationService;
    
    public NotificationController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllNotifications()
    {
        var result = await _notificationService.GetAllForUserAsync();
        return Ok(result.Value);
    }

    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadNotifications()
    {
        var result = await _notificationService.GetAllUnreadForUserAsync();
        return Ok(result.Value);
    }
    
    [HttpPut("{notificationId}/read")]
    public async Task<IActionResult> MarkAsRead(int notificationId)
    {
        var result = await _notificationService.MarkAsReadAsync(notificationId);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}