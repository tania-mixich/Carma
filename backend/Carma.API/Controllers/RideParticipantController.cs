using Carma.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carma.API.Controllers;

[ApiController]
[Route("rides/{rideId}/participants")]
[Authorize]
public class RideParticipantController : ControllerBase
{
    private readonly RideParticipantService  _rideParticipantService;

    public RideParticipantController(RideParticipantService rideParticipantService)
    {
        _rideParticipantService = rideParticipantService;
    }

    [HttpPost]
    public async Task<IActionResult> RequestToJoin(int rideId)
    {
        var result = await _rideParticipantService.RequestToJoinRideAsync(rideId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{userId:guid}/accept")]
    public async Task<IActionResult> AcceptParticipant(int rideId, Guid userId)
    {
        var result = await _rideParticipantService.AcceptRideParticipantAsync(rideId, userId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> RemoveParticipant(int rideId, Guid userId)
    {
        var result = await _rideParticipantService.RejectRideParticipantAsync(rideId, userId);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
    
    [HttpDelete("leave")]
    public async Task<IActionResult> LeaveRide(int rideId)
    {
        var result = await _rideParticipantService.LeaveRideAsync(rideId);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
    
}