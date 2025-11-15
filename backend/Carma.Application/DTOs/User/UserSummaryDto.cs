namespace Carma.Application.DTOs.User;

public record UserSummaryDto(
    Guid Id,
    string UserName,
    string? ImageUrl,
    double Karma
    );