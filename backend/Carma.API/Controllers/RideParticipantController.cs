using Carma.API.Extensions;
using Carma.Application.DTOs.RideParticipant;
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

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(RequestToJoin), new { rideId }, result.Value);
        }
        return result.ToActionResult();
    }

    [HttpPatch("{userId:guid}")]
    public async Task<IActionResult> AcceptParticipant(int rideId, Guid userId, [FromBody] RideParticipantUpdateDto dto)
    {
        var result = await _rideParticipantService.HandleRideParticipantAsync(rideId, userId, dto);
        return result.ToActionResult();
    }
    
    [HttpPatch("me")]
    public async Task<IActionResult> LeaveRide(int rideId)
    {
        var result = await _rideParticipantService.LeaveRideAsync(rideId);
        return result.ToActionResult();
    }
    
}