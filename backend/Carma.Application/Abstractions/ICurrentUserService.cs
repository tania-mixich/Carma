namespace Carma.Application.Abstractions;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string Email { get; }
    string Username { get; }
}