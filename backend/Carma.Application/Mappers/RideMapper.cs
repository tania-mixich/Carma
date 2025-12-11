using Carma.Application.DTOs.Location;
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
    
    public static RideGetDto MapToGetDto(RideLookupDto raw)
    {
        return new RideGetDto(
            raw.Id,
            raw.OrganizerName,
            raw.OrganizerKarma,
            raw.OrganizerImageUrl ?? string.Empty,
            new LocationGetDto(raw.PickupPoint.Y, raw.PickupPoint.X),
            new LocationGetDto(raw.DropoffPoint.Y, raw.DropoffPoint.X),
            raw.PickupTime,
            raw.AcceptedCount > 0 ? raw.Price / raw.AcceptedCount : raw.Price,
            raw.Seats - raw.AcceptedCount,
            raw.Status.ToString()
        );
    }
}