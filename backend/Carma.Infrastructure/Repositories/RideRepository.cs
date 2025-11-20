using Carma.Application.Abstractions;
using Carma.Application.Abstractions.Repositories;
using Carma.Application.DTOs.Location;
using Carma.Application.DTOs.Ride;
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

    public async Task<IEnumerable<RideGetDto>> GetNearbyRidesAsync(Point startLocation, int radius)
    {
        var rides = await _context.Rides
            .AsNoTracking()
            .Where(r => r.Status == Status.Available &&
                        r.PickupLocation.Coordinate.IsWithinDistance(startLocation, radius))
            .Select(r => new
            {
                r.Id,
                OrganizerName = r.Organizer.UserName,
                PickupPoint = r.PickupLocation.Coordinate,
                DropOffPoint = r.DropOffLocation.Coordinate,
                r.PickupTime,
                r.Price,
                r.Seats,
                r.Status,
                AcceptedCount = r.Participants.Count(rp => rp.Status == ParticipantStatus.Accepted)
            })
            .ToListAsync();

        return rides.Select(r => new RideGetDto(
                r.Id,
                r.OrganizerName,
                new LocationGetDto(r.PickupPoint.Y, r.PickupPoint.X),
                new LocationGetDto(r.DropOffPoint.Y, r.DropOffPoint.X),
                r.PickupTime,
                r.AcceptedCount > 0 ? r.Price / r.AcceptedCount : r.Price,
                r.Seats - r.AcceptedCount,
                r.Status.ToString()
                )
            );
    }

    public async Task<IEnumerable<RideGetDto>> GetNearbyRidesHeadingToTheLocationAsync(Point startLocation, int startRadius, Point endLocation,
        int endRadius)
    {
        var rides = await _context.Rides
            .AsNoTracking()
            .Where(r => r.Status == Status.Available && 
                        r.PickupLocation.Coordinate.IsWithinDistance(startLocation, startRadius) &&
                        r.DropOffLocation.Coordinate.IsWithinDistance(endLocation, endRadius))
            .Select(r => new
            {
                r.Id,
                OrganizerName = r.Organizer.UserName,
                PickupPoint = r.PickupLocation.Coordinate,
                DropOffPoint = r.DropOffLocation.Coordinate,
                r.PickupTime,
                r.Price,
                r.Seats,
                r.Status,
                AcceptedCount = r.Participants.Count(rp => rp.Status == ParticipantStatus.Accepted)
            })
            .ToListAsync();

        return rides.Select(r => new RideGetDto(
                r.Id,
                r.OrganizerName,
                new LocationGetDto(r.PickupPoint.Y, r.PickupPoint.X),
                new LocationGetDto(r.DropOffPoint.Y, r.DropOffPoint.X),
                r.PickupTime,
                r.AcceptedCount > 0 ? r.Price / r.AcceptedCount : r.Price,
                r.Seats - r.AcceptedCount,
                r.Status.ToString()
            )
        );
    }
}