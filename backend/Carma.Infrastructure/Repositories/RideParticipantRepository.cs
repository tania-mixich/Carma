using Carma.Application.Abstractions;
using Carma.Application.Abstractions.Repositories;
using Carma.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Carma.Infrastructure.Repositories;

public class RideParticipantRepository : IRideParticipantRepository
{
    private readonly CarmaDbContext _context;
    public RideParticipantRepository(CarmaDbContext context)
    {
        _context = context;
    }
    
    public async Task<RideParticipant?> GetByRideAndUserAsync(int rideId, Guid rideParticipantId)
    {
        return await _context.RideParticipants
            .Include(rp => rp.Ride)
            .Include(rp => rp.User)
            .FirstOrDefaultAsync(rp => rp.RideId == rideId && rp.UserId == rideParticipantId);
    }
    
    public async Task<RideParticipant> AddAsync(RideParticipant rideParticipant)
    {
        await _context.RideParticipants.AddAsync(rideParticipant);
        return rideParticipant;
    }
    
    public void Update(RideParticipant entity)
    {
        _context.RideParticipants.Update(entity);
    }
    
    public void Delete(RideParticipant entity)
    {
        _context.RideParticipants.Remove(entity);
    }

    public async Task<bool> ContainsUserAsync(int rideId, Guid userId)
    {
        return await _context.RideParticipants.AnyAsync(rp => rp.UserId == userId && rp.RideId == rideId);
    }
}