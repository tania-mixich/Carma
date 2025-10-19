using Carma.Application.DTOs.Auth;
using Carma.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Carma.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterRequestDto requestDto)
    {
        var result = await _authService.RegisterAsync(requestDto);
        return result.IsSuccess ? Ok(result) : BadRequest(result); 
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginRequestDto requestDto)
    {
        var result = await _authService.LoginAsync(requestDto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}