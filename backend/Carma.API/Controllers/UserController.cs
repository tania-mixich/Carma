using Carma.Application.DTOs.User;
using Carma.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carma.API.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    
    public UserController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userService.GetAllAsync();
        return Ok(result.Value);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        var result = await _userService.GetProfileAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPatch("self")]
    public async Task<IActionResult> Update(UserUpdateDto userUpdateDto)
    {
        var result = await _userService.UpdateProfileAsync(userUpdateDto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
    
    [HttpDelete("self")]
    public async Task<IActionResult> Delete()
    {
        var result = await _userService.DeleteAsync();
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}