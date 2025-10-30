using Carma.Application.DTOs.Location;
using Carma.Domain.Enums;

namespace Carma.Application.DTOs.Ride;

public record RideGetDto(
    int Id,
    string OrganizerName,
    LocationGetDto PickupLocation,
    LocationGetDto DropOffLocation,
    DateTime PickupTime,
    double PricePerSeat,
    int AvailableSeats,
    string Status
);