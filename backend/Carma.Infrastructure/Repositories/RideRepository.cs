using Carma.Application.Abstractions;
using Carma.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Carma.Infrastructure.Repositories;

public class RideRepository : GenericRepository<Ride>, IRideRepository
{
    private readonly CarmaDbContext _context;
    public RideRepository(CarmaDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ride>> GetAllAsync()
    {
        return await _context.Rides.ToListAsync();
    }
}