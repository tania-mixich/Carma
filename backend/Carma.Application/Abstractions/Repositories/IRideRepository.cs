using Carma.Domain.Entities;
using NetTopologySuite.Geometries;

namespace Carma.Application.Abstractions.Repositories;

public interface IRideRepository 
{
    Task<IEnumerable<Ride>> GetNearbyRidesAsync(Point startLocation, int radius);
    Task<IEnumerable<Ride>> GetNearbyRidesHeadingToTheLocationAsync(Point startLocation, int startRadius, Point endLocation, int endRadius);
}