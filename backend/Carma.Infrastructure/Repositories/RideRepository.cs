using Carma.Application.Abstractions.Repositories;
using Carma.Application.DTOs.Location;
using Carma.Application.DTOs.Ride;
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

    public async Task<IEnumerable<RideGetDto>> GetNearbyRidesAsync(Point startLocation, int radius, Guid userId)
    {
        var rides = await _context.Rides
            .AsNoTracking()
            .Where(r => r.Status == Status.Available &&
                        r.PickupTime > DateTime.UtcNow &&
                        r.PickupLocation.Coordinate.IsWithinDistance(startLocation, radius))
            .Select(r => new
            {
                r.Id,
                OrganizerName = r.Organizer.UserName,
                OrganizerKarma = r.Organizer.Karma,
                OrganizerImage = r.Organizer.ImageUrl,
                PickupPoint = r.PickupLocation.Coordinate,
                PickupAddress = r.PickupLocation.Address,
                PickupCity = r.PickupLocation.City,
                PickupCountry = r.PickupLocation.Country,
                DropOffPoint = r.DropOffLocation.Coordinate,
                DropOffAddress = r.DropOffLocation.Address,
                DropOffCity = r.DropOffLocation.City,
                DropOffCountry = r.DropOffLocation.Country,
                r.PickupTime,
                r.Price,
                r.Seats,
                r.Status,
                AcceptedCount = r.Participants.Count(rp => rp.Status == ParticipantStatus.Accepted),
                UserStatus = r.OrganizerId == userId ? "Organizer" : 
                    r.Participants
                    .Where(p => p.UserId == userId)
                    .Select(p => p.Status.ToString())
                    .FirstOrDefault() ?? "None",
            })
            .ToListAsync();

        return rides.Select(r => new RideGetDto(
                r.Id,
                r.OrganizerName,
                r.OrganizerKarma,
                r.OrganizerImage ?? string.Empty,
                new LocationGetDto(
                    r.PickupPoint.Y, 
                    r.PickupPoint.X, 
                    r.PickupAddress,
                    r.PickupCity ?? string.Empty,
                    r.PickupCountry ?? string.Empty
                    ),
                new LocationGetDto(
                    r.DropOffPoint.Y, 
                    r.DropOffPoint.X, 
                    r.DropOffAddress, 
                    r.DropOffCity ?? string.Empty, 
                    r.DropOffCountry ?? string.Empty
                    ),
                r.PickupTime,
                r.AcceptedCount > 0 ? r.Price / r.AcceptedCount : r.Price,
                r.Seats - r.AcceptedCount,
                r.Status.ToString(),
                r.UserStatus
                )
            );
    }

    public async Task<IEnumerable<RideGetDto>> GetNearbyRidesHeadingToTheLocationAsync(Point startLocation, int startRadius, Point endLocation,
        int endRadius, Guid userId)
    {
        var rides = await _context.Rides
            .AsNoTracking()
            .Where(r => r.Status == Status.Available && 
                        r.PickupTime > DateTime.UtcNow &&
                        r.PickupLocation.Coordinate.IsWithinDistance(startLocation, startRadius) &&
                        r.DropOffLocation.Coordinate.IsWithinDistance(endLocation, endRadius))
            .Select(r => new
            {
                r.Id,
                OrganizerName = r.Organizer.UserName,
                OrganizerKarma = r.Organizer.Karma,
                OrganizerImage = r.Organizer.ImageUrl,
                PickupPoint = r.PickupLocation.Coordinate,
                PickUpAddress = r.PickupLocation.Address,
                PickUpCity = r.PickupLocation.City,
                PickUpCountry = r.PickupLocation.Country,
                DropOffPoint = r.DropOffLocation.Coordinate,
                DropOffAddress = r.DropOffLocation.Address,
                DropOffCity = r.DropOffLocation.City,
                DropOffCountry = r.DropOffLocation.Country,
                r.PickupTime,
                r.Price,
                r.Seats,
                r.Status,
                AcceptedCount = r.Participants.Count(rp => rp.Status == ParticipantStatus.Accepted),
                UserStatus = r.OrganizerId == userId ? "Organizer" : 
                    r.Participants
                        .Where(p => p.UserId == userId)
                        .Select(p => p.Status.ToString())
                        .FirstOrDefault() ?? "None",
            })
            .ToListAsync();

        return rides.Select(r => new RideGetDto(
                r.Id,
                r.OrganizerName,
                r.OrganizerKarma,
                r.OrganizerImage ?? string.Empty,
                new LocationGetDto(
                    r.PickupPoint.Y, 
                    r.PickupPoint.X,
                    r.PickUpAddress, 
                    r.PickUpCity ?? string.Empty, 
                    r.PickUpCountry ?? string.Empty
                    ),
                new LocationGetDto(
                    r.DropOffPoint.Y, 
                    r.DropOffPoint.X,
                    r.DropOffAddress, 
                    r.DropOffCity ?? string.Empty,
                    r.DropOffCountry ?? string.Empty
                    ),
                r.PickupTime,
                r.AcceptedCount > 0 ? r.Price / r.AcceptedCount : r.Price,
                r.Seats - r.AcceptedCount,
                r.Status.ToString(),
                r.UserStatus
            )
        );
    }
}