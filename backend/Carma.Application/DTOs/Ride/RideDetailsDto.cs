using Carma.Application.DTOs.Location;
using Carma.Application.DTOs.RideParticipant;
using Carma.Domain.Enums;

namespace Carma.Application.DTOs.Ride;

public record RideDetailsDto(
    LocationGetDto PickupLocation,
    LocationGetDto DropOffLocation,
    DateTime PickupTime,
    double Price,
    int AvailableSeats,
    string Status,
    List<RideParticipantGetDto> Participants
    );