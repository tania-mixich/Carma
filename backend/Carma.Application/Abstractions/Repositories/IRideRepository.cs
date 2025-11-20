using Carma.Application.DTOs.Ride;
using Carma.Domain.Entities;
using NetTopologySuite.Geometries;

namespace Carma.Application.Abstractions.Repositories;

public interface IRideRepository 
{
    Task<IEnumerable<RideGetDto>> GetNearbyRidesAsync(Point startLocation, int radius);
    Task<IEnumerable<RideGetDto>> GetNearbyRidesHeadingToTheLocationAsync(Point startLocation, int startRadius, Point endLocation, int endRadius);
}