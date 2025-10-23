using Carma.Application.DTOs.Ride;
using Carma.Domain.Entities;

namespace Carma.Application.Mappers;

public static class RideMapper
{
    public static Ride MapToRide(RideCreateDto rideCreateDto, User organizer)
    {
        return new Ride
        {
            CreatedAt = rideCreateDto.CreatedAt,
            UpdatedAt = rideCreateDto.UpdatedAt,
            PickupLocation = rideCreateDto.PickupLocation,
            DropOffLocation = rideCreateDto.DropOffLocation,
            PickupTime = rideCreateDto.PickupTime,
            Price = rideCreateDto.Price,
            AvailableSeats = rideCreateDto.AvailableSeats,
            Status = rideCreateDto.Status,
            Organizer = organizer
        };
    }

    public static RideGetDto MapToRideGetDto(Ride ride)
    {
        return new RideGetDto
        (
            ride.CreatedAt,
            ride.UpdatedAt,
            ride.PickupLocation,
            ride.DropOffLocation,
            ride.PickupTime,
            ride.Price,
            ride.AvailableSeats,
            ride.Status
        );
    }
}