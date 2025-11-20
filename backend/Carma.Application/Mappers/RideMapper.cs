using Carma.Application.DTOs.Ride;
using Carma.Domain.Entities;

namespace Carma.Application.Mappers;

public static class RideMapper
{
    public static Ride MapToRide(RideCreateDto rideCreateDto)
    {
        return new Ride
        {
            PickupLocation = LocationMapper.MapToLocation(rideCreateDto.PickupLocation),
            DropOffLocation = LocationMapper.MapToLocation(rideCreateDto.DropOffLocation),
            PickupTime = rideCreateDto.PickupTime,
            Price = rideCreateDto.Price,
            Seats = rideCreateDto.AvailableSeats
        };
    }
}