using Carma.Application.DTOs.Location;
using Carma.Domain.ValueObjects;

namespace Carma.Application.Mappers;

public static class LocationMapper
{
    public static Location MapToLocation(LocationCreateDto locationCreateDto)
    {
        return new Location(locationCreateDto.Latitude, locationCreateDto.Longitude);
    }
}