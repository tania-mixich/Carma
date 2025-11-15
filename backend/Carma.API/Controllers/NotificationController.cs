using Carma.API.Extensions;
using Carma.Application.DTOs.Notification;
using Carma.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carma.API.Controllers;

[ApiController]
[Route("/notifications")]
[Authorize]
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
        return result.ToActionResult();
    }

    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadNotifications()
    {
        var result = await _notificationService.GetAllUnreadForUserAsync();
        return result.ToActionResult();
    }
    
    [HttpPatch("{notificationId}")]
    public async Task<IActionResult> MarkAsRead(int notificationId, [FromBody] NotificationUpdateDto notificationUpdateDto)
    {
        var result = await _notificationService.MarkAsReadAsync(notificationId, notificationUpdateDto);
        return result.ToActionResult();
    }
}