namespace Carma.Application.DTOs.User;

public record UserProfileDto(
    Guid Id,
    string UserName,
    string? ImageUrl,
    double Karma,
    int RidesCount
    );