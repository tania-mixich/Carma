namespace Carma.Application.DTOs.User;

public record UserSummaryDto(
    string UserName,
    string? ImageUrl,
    int Karma
    );