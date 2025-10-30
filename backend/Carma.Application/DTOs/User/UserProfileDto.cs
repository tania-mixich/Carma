namespace Carma.Application.DTOs.User;

public record UserProfileDto(
    string UserName,
    string? ImageUrl,
    double Karma,
    int RidesCount
    );