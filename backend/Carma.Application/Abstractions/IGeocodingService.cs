using Carma.Domain.ValueObjects;

namespace Carma.Application.Abstractions;

public interface IGeocodingService
{
    Task<Location> GetLocationFromCoordinatesAsync(double latitude, double longitude);
}