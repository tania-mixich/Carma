using Carma.Application.DTOs.Ride;
using Carma.Application.Services;
using Carma.Domain.Enums;
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

    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearbyRides(int distance = 1000)
    {
        var result = await _rideService.GetNearbyRidesAsync(distance);
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

    [HttpPatch("{rideId}")]
    public async Task<IActionResult> Update(int rideId, RideUpdateDto rideUpdateDto)
    {
        var result = await _rideService.UpdateRideAsync(rideId, rideUpdateDto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPatch("{rideId}/status")]
    public async Task<IActionResult> ChangeStatus(int rideId, Status status)
    {
        var result = await _rideService.UpdateRideStatusAsync(rideId, status);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}