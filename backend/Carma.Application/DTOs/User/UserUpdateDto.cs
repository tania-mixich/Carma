using Carma.Application.DTOs.Location;

namespace Carma.Application.DTOs.User;

public record UserUpdateDto(
    string? UserName,
    string? ImageUrl,
    LocationCreateDto? Location
    );