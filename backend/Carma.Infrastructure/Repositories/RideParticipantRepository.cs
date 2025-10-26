using Carma.Application.Abstractions;
using Carma.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Carma.Infrastructure.Repositories;

public class RideParticipantRepository : GenericRepository<RideParticipant>, IRideParticipantRepository
{
    private readonly CarmaDbContext _context;
    public RideParticipantRepository(CarmaDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RideParticipant>> GetAllByRideIdAsync(int rideId)
    {
        return await _context.RideParticipants
            .Include(rp => rp.User)
            .Where(rp => rp.RideId == rideId)
            .ToListAsync();
    }
    
    public async Task<RideParticipant?> GetByRideAndUserAsync(int rideId, Guid rideParticipantId)
    {
        return await _context.RideParticipants
            .Include(rp => rp.Ride)
            .Include(rp => rp.User)
            .FirstOrDefaultAsync(rp => rp.RideId == rideId && rp.UserId == rideParticipantId);
    }
}