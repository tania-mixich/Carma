namespace Carma.Application.DTOs.Review;

public record ReviewCreateDto(
    int RideId,
    double Karma,
    string Text
    );