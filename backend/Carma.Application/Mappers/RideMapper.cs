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
            PricePerSeat = rideCreateDto.Price,
            AvailableSeats = rideCreateDto.AvailableSeats
        };
    }

    public static RideGetDto MapToRideGetDto(Ride ride)
    {
        return new RideGetDto
        (
            ride.Id,
            ride.Organizer.UserName,
            LocationMapper.MapToLocationGetDto(ride.PickupLocation),
            LocationMapper.MapToLocationGetDto(ride.DropOffLocation),
            ride.PickupTime,
            ride.PricePerSeat,
            ride.AvailableSeats,
            ride.Status.ToString()
        );
    }

    public static RideDetailsDto MapToRideDetailsDto(Ride ride)
    {
        return new RideDetailsDto(
            LocationMapper.MapToLocationGetDto(ride.PickupLocation),
            LocationMapper.MapToLocationGetDto(ride.DropOffLocation),
            ride.PickupTime,
            ride.PricePerSeat,
            ride.AvailableSeats,
            ride.Status.ToString(),
            ride.Participants.Select(RideParticipantMapper.MapToRideParticipantGetDto).ToList()
            );
    }
}