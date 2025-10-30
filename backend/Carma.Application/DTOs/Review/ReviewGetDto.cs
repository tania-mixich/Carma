using Carma.Application.DTOs.User;

namespace Carma.Application.DTOs.Review;

public record ReviewGetDto(
    UserSummaryDto Reviewer,
    int RideId,
    double Karma,
    string Text
    );