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
    
    public async Task<IEnumerable<Ride>> GetAllAsync()
    {
        return await _context.Rides
            .Include(r => r.Organizer)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ride>> GetNearbyRidesAsync(Point userLocation, int radius)
    {
        return await _context.Rides
            .Include(r => r.Participants)
                .ThenInclude(rp => rp.User)
            .Where(r => r.Status == Status.Available &&
                        r.PickupLocation.Coordinate.IsWithinDistance(userLocation, radius))
            .ToListAsync();
    }
    
    public async Task<Ride?> GetByIdAsync(int id)
    {
        return await _context.Rides
            .Include(r => r.Participants.Where(rp => rp.IsAccepted))
                .ThenInclude(rp => rp.User)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
    
    public async Task<Ride> AddAsync(Ride ride)
    {
        await _context.Rides.AddAsync(ride);
        return ride;
    }
    
    public void Update(Ride entity)
    {
        _context.Rides.Update(entity);
    }
    
    public void Delete(Ride entity)
    {
        _context.Rides.Remove(entity);
    }
}