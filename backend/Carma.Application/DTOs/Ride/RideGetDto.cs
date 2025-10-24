using Carma.Application.DTOs.Location;
using Carma.Domain.Enums;

namespace Carma.Application.DTOs.Ride;

public record RideGetDto(
    int Id,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    LocationGetDto PickupLocation,
    LocationGetDto DropOffLocation,
    DateTime PickupTime,
    double Price,
    int AvailableSeats,
    Status Status
);