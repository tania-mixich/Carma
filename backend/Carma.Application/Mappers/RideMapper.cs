using Carma.Application.DTOs.Location;
using Carma.Application.DTOs.Ride;
using Carma.Domain.Entities;
using Carma.Domain.ValueObjects;

namespace Carma.Application.Mappers;

public static class RideMapper
{
    public static Ride MapToRide(RideCreateDto rideCreateDto, Location pickupLocation, Location dropOffLocation)
    {
        return new Ride
        {
            PickupLocation = pickupLocation,
            DropOffLocation = dropOffLocation,
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
            new LocationGetDto(
                raw.PickupPoint.Y, 
                raw.PickupPoint.X, 
                raw.PickupAddress, 
                raw.PickupCity ?? string.Empty, 
                raw.PickupCountry ?? string.Empty
                ),
            new LocationGetDto(
                raw.DropoffPoint.Y, 
                raw.DropoffPoint.X, 
                raw.DropoffAddress, 
                raw.DropoffCity ?? string.Empty, 
                raw.DropoffCountry ?? string.Empty
                ),
            raw.PickupTime,
            raw.AcceptedCount > 0 ? raw.Price / raw.AcceptedCount : raw.Price,
            raw.Seats - raw.AcceptedCount,
            raw.Status.ToString()
        );
    }
}