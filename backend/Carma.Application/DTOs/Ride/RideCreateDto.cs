using Carma.Domain.Enums;
using Carma.Domain.ValueObjects;

namespace Carma.Application.DTOs.Ride;

public record RideCreateDto(
    DateTime CreatedAt,
    DateTime UpdatedAt,
    Location PickupLocation,
    Location DropOffLocation,
    DateTime PickupTime,
    double Price,
    int AvailableSeats,
    Status Status
    );