using Carma.Application.DTOs.Location;

namespace Carma.Application.DTOs.Ride;

public record RideGetDto(
    int Id,
    string OrganizerName,
    double Karma,
    string ImageUrl,
    LocationGetDto PickupLocation,
    LocationGetDto DropOffLocation,
    DateTime PickupTime,
    double PricePerSeat,
    int AvailableSeats,
    string Status
);