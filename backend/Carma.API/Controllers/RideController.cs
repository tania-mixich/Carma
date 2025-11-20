using Carma.API.Extensions;
using Carma.Application.DTOs.Location;
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
        return result.ToActionResult();
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        var result = await _rideService.GetPreviousRides();
        return result.ToActionResult();
    }

    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearbyRides([FromQuery] RideQueryDto query)
    {
        var result = await _rideService.GetNearbyRidesAsync(query);
        return result.ToActionResult();
    }

    [HttpGet("{rideId}")]
    public async Task<IActionResult> GetById(int rideId)
    {
        var result = await _rideService.GetRideAsync(rideId);
        return result.ToActionResult();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(RideCreateDto rideCreateDto)
    {
        var result = await _rideService.CreateRideAsync(rideCreateDto);
        return result.ToActionResult();
    }

    [HttpPatch("{rideId}")]
    public async Task<IActionResult> Update(int rideId, RideUpdateDto rideUpdateDto)
    {
        var result = await _rideService.UpdateRideAsync(rideId, rideUpdateDto);
        return result.ToActionResult();
    }

    [HttpPatch("{rideId}/status")]
    public async Task<IActionResult> ChangeStatus(int rideId, [FromBody] RideStatusUpdateDto rideStatusUpdateDto)
    {
        var result = await _rideService.UpdateRideStatusAsync(rideId, rideStatusUpdateDto);
        return result.ToActionResult();
    }
}