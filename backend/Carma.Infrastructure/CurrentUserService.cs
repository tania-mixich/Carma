using System.Security.Claims;
using Carma.Application.Abstractions;
using Carma.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Carma.Infrastructure;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId => Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new NotFoundException("User not found."));
    public string Email => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email) ?? throw new NotFoundException("User not found.");
    public string Username => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name) ?? throw new NotFoundException("User not found.");
}