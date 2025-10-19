namespace Carma.Application.Abstractions;

public interface IJwtService
{
    string GenerateToken(Guid userId, string email);
}