using Carma.Application.DTOs.Location;
using Carma.Domain.ValueObjects;

namespace Carma.Application.Mappers;

public static class LocationMapper
{
    public static Location MapToLocation(LocationCreateDto locationCreateDto)
    {
        return new Location(locationCreateDto.Latitude, locationCreateDto.Longitude);
    }

    public static Location MapToLocation(LocationGetDto locationGetDto)
    {
        return new Location(locationGetDto.Latitude, locationGetDto.Longitude);
    }

    public static LocationGetDto MapToLocationGetDto(Location location)
    {
        return new LocationGetDto(location.Coordinate.Y, location.Coordinate.X, location.Address);
    }
}