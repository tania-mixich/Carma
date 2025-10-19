using Carma.Application.Abstractions;
using Carma.Application.Common;
using Carma.Application.DTOs.Auth;
using Carma.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Carma.Application.Services;

public class AuthService
{
    private readonly IJwtService _jwtService;
    private readonly UserManager<User> _userManager;
    
    public AuthService(IJwtService jwtService, UserManager<User> userManager)
    {
        _jwtService = jwtService;
        _userManager = userManager;
    }

    public async Task<Result<string>> RegisterAsync(RegisterRequestDto requestDto)
    {
        if (requestDto.Password != requestDto.ConfirmPassword)
        {
            return Result<string>.Failure("Passwords do not match");
        }
        
        var user = new User
        {
            Email = requestDto.Email,
            EmailConfirmed = true,
            UserName = requestDto.UserName,
            Karma = 0
        };
        
        var createResult = await _userManager.CreateAsync(user, requestDto.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
            return Result<string>.Failure(errors);
        }
        var token = _jwtService.GenerateToken(user.Id, user.Email);
        
        return Result<string>.Success(token);
    }

    public async Task<Result<string>> LoginAsync(LoginRequestDto requestDto)
    {
        var user = await _userManager.FindByEmailAsync(requestDto.Email);
        if (user == null)
        {
            return Result<string>.Failure("User not found");
        }

        await _userManager.CheckPasswordAsync(user, requestDto.Password);
        var token = _jwtService.GenerateToken(user.Id, user.Email!);
        
        return Result<string>.Success(token);
    }
    
    
}