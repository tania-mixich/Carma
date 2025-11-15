using Carma.Application.Abstractions;
using Carma.Application.Common;
using Carma.Application.DTOs.User;
using Carma.Application.Mappers;
using Carma.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Carma.Application.Services;

public class UserService
{
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<UserUpdateDto> _updateValidator;

    public UserService(UserManager<User> userManager, ICurrentUserService currentUserService, IValidator<UserUpdateDto> updateValidator)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
        _updateValidator = updateValidator;
    }

    public async Task<Result<IEnumerable<UserSummaryDto>>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        return Result<IEnumerable<UserSummaryDto>>.Success(users.Select(u => new UserSummaryDto(
            u.Id,
            u.UserName,
            u.ImageUrl,
            u.Karma
            ))
        );
    }

    public async Task<Result<object>> GetProfileAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result<object>.NotFound("User not found");
        }

        if (userId == _currentUserService.UserId)
        {
            return Result<object>.Success(new UserSelfDto(
                userId,
                user.UserName,
                user.Email,
                user.ImageUrl,
                user.Karma,
                user.RidesCount,
                user.CreatedAt
                )
            );
        }
        
        return Result<object>.Success(new UserProfileDto(
            userId,
            user.UserName,
            user.ImageUrl,
            user.Karma,
            user.RidesCount
            )
        );
    }

    public async Task<Result<UserSelfDto>> UpdateProfileAsync(UserUpdateDto userUpdateDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(userUpdateDto);
        if (!validationResult.IsValid)
        {
            return Result<UserSelfDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).First());
        }
        
        var user = await _userManager.Users
            .Include(u => u.Location)
            .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId);
        if (user == null)
        {
            return Result<UserSelfDto>.NotFound("User not found");
        }
        
        user.UserName = userUpdateDto.UserName ?? user.UserName;
        user.ImageUrl = userUpdateDto.ImageUrl ?? user.ImageUrl;
        user.Location = userUpdateDto.Location != null ? LocationMapper.MapToLocation(userUpdateDto.Location) : user.Location;
        user.UpdatedAt = DateTime.UtcNow;
        
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return Result<UserSelfDto>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));
        }
        
        return Result<UserSelfDto>.Success(new UserSelfDto(
                user.Id,
                user.UserName,
                user.Email,
                user.ImageUrl,
                user.Karma,
                user.RidesCount,
                user.CreatedAt
            )
        );
    }

    public async Task<Result> DeleteAsync()
    {
        var user = await _userManager.FindByIdAsync(_currentUserService.UserId.ToString());
        if (user == null)
        {
            return Result.NotFound("User not found");
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return Result.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));
        }
        
        return Result.Success();
    }
}