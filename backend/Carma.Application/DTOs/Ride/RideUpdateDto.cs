using Carma.Application.DTOs.Location;

namespace Carma.Application.DTOs.Ride;

public record RideUpdateDto(
    LocationCreateDto PickupLocation,
    LocationCreateDto DropOffLocation,
    DateTime PickupTime,
    double Price,
    int AvailableSeats);