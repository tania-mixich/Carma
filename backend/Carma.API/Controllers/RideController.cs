using Carma.Application.DTOs.Ride;
using Carma.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carma.API.Controllers;

[ApiController]
[Route("rides")]
[Authorize]
public class RideController : ControllerBase
{
    private readonly RideService _rideService;

    public RideController(RideService rideService)
    {
        _rideService = rideService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _rideService.GetAllRidesAsync();
        return Ok(result.Value);
    }

    [HttpGet("{rideId}")]
    public async Task<IActionResult> GetById(int rideId)
    {
        var result = await _rideService.GetRideAsync(rideId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create(RideCreateDto rideCreateDto)
    {
        var result = await _rideService.CreateRideAsync(rideCreateDto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{rideId}")]
    public async Task<IActionResult> Update(int rideId, RideUpdateDto rideUpdateDto)
    {
        var result = await _rideService.UpdateRideAsync(rideId, rideUpdateDto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
    
    [HttpDelete("{rideId}")]
    public async Task<IActionResult> Delete(int rideId)
    {
        var result = await _rideService.DeleteRideAsync(rideId);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}