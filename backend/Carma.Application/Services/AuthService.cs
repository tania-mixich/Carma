using Carma.Application.Abstractions;
using Carma.Application.Common;
using Carma.Application.DTOs.Auth;
using Carma.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Carma.Application.Services;

public class AuthService
{
    private readonly IJwtService _jwtService;
    private readonly UserManager<User> _userManager;
    private readonly IValidator<RegisterRequestDto> _registerValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;
    
    public AuthService(IJwtService jwtService, UserManager<User> userManager, IValidator<RegisterRequestDto> registerValidator, IValidator<LoginRequestDto> loginValidator)
    {
        _jwtService = jwtService;
        _userManager = userManager;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto requestDto)
    {
        var validationResult = await _registerValidator.ValidateAsync(requestDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result<AuthResponseDto>.Failure(string.Join("; ", errors));
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
            var errors = createResult.Errors.Select(e => e.Description);
            return Result<AuthResponseDto>.Failure(string.Join("; ", errors));
        }
        var token = _jwtService.GenerateToken(user.Id, user.Email, user.UserName);
        
        return Result<AuthResponseDto>.Success(new AuthResponseDto(token));
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto requestDto)
    {
        var validationResult = await _loginValidator.ValidateAsync(requestDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result<AuthResponseDto>.Failure(string.Join("; ", errors));
        }
        
        var user = await _userManager.FindByEmailAsync(requestDto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, requestDto.Password))
        {
            return Result<AuthResponseDto>.Failure("Invalid credentials");
        }
        
        var token = _jwtService.GenerateToken(user.Id, user.Email!, user.UserName!);
        
        return Result<AuthResponseDto>.Success(new AuthResponseDto(token));
    }
    
    
}