namespace Carma.Application.DTOs.User;

public record UserProfileDto(
    string UserName,
    string? ImageUrl,
    int Karma,
    int RidesCount
    );