namespace Carma.Application.DTOs.User;

public record UserSelfDto(
    string UserName,
    string Email,
    string? ImageUrl,
    double Karma,
    int RidesCount,
    DateTime CreatedAt
    );