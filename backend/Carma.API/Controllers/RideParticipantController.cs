using Carma.Application.DTOs.RideParticipant;
using Carma.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Carma.API.Controllers;

[ApiController]
public class RideParticipantController : ControllerBase
{
    private readonly RideParticipantService  _rideParticipantService;

    public RideParticipantController(RideParticipantService rideParticipantService)
    {
        _rideParticipantService = rideParticipantService;
    }

    [HttpGet("rides/{rideId}/participants")]
    public async Task<IActionResult> GetAllByRideId(int rideId)
    {
        var result = await _rideParticipantService.GetParticipantsOfRideAsync(rideId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("rides/{rideId}/participants/{userId}")]
    public async Task<IActionResult> GetByRideAndUserId(int rideId, Guid userId)
    {
        var result = await _rideParticipantService.GetParticipantByRideAndUserAsync(rideId, userId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("rides/{rideId}/participants")]
    public async Task<IActionResult> Create(int rideId)
    {
        var result = await _rideParticipantService.CreateRideParticipantAsync(rideId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("rides/{rideId}/participants/{userId}")]
    public async Task<IActionResult> Update(int rideId, Guid userId, RideParticipantUpdateDto rideParticipantUpdateDto)
    {
        var result = await _rideParticipantService.UpdateRideParticipantAsync(rideId, userId, rideParticipantUpdateDto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("rides/{rideId}/participants/{userId}")]
    public async Task<IActionResult> Delete(int rideId, Guid userId)
    {
        var result = await _rideParticipantService.DeleteRideParticipantAsync(rideId, userId);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}