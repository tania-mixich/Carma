using Carma.Application.Abstractions;
using Carma.Application.Abstractions.Repositories;
using Carma.Domain.Entities;
using Carma.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Carma.Infrastructure.Repositories;

public class RideRepository : IRideRepository
{
    private readonly CarmaDbContext _context;
    public RideRepository(CarmaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ride>> GetNearbyRidesAsync(Point startLocation, int radius)
    {
        return await _context.Rides
            .Include(r => r.Participants)
                .ThenInclude(rp => rp.User)
            .Where(r => r.Status == Status.Available &&
                        r.PickupLocation.Coordinate.IsWithinDistance(startLocation, radius))
            .ToListAsync();
    }

    public async Task<IEnumerable<Ride>> GetNearbyRidesHeadingToTheLocationAsync(Point startLocation, int startRadius, Point endLocation,
        int endRadius)
    {
        return await _context.Rides
            .Include(r => r.Participants)
                .ThenInclude(rp => rp.User)
            .Where(r => r.Status == Status.Available && 
                        r.PickupLocation.Coordinate.IsWithinDistance(startLocation, startRadius) &&
                        r.DropOffLocation.Coordinate.IsWithinDistance(endLocation, endRadius))
            .ToListAsync();
    }
}