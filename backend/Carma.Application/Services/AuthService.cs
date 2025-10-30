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

    public async Task<Result<string>> RegisterAsync(RegisterRequestDto requestDto)
    {
        var validationResult = await _registerValidator.ValidateAsync(requestDto);
        if (!validationResult.IsValid)
        {
            return Result<string>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).First());
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
        var token = _jwtService.GenerateToken(user.Id, user.Email, user.UserName);
        
        return Result<string>.Success(token);
    }

    public async Task<Result<string>> LoginAsync(LoginRequestDto requestDto)
    {
        var validationResult = await _loginValidator.ValidateAsync(requestDto);
        if (!validationResult.IsValid)
        {
            return Result<string>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).First());
        }
        
        var user = await _userManager.FindByEmailAsync(requestDto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, requestDto.Password))
        {
            return Result<string>.Failure("Invalid credentials");
        }
        
        var token = _jwtService.GenerateToken(user.Id, user.Email!, user.UserName!);
        
        return Result<string>.Success(token);
    }
    
    
}