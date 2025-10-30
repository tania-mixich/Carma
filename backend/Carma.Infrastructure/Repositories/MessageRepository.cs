using Carma.Application.Abstractions.Repositories;
using Carma.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Carma.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly CarmaDbContext _context;
    
    public MessageRepository(CarmaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Message>> GetAllFromRideAsync(int rideId)
    {
        return await _context.Messages
            .Include(m => m.User)
            .Where(m => m.RideId == rideId)
            .ToListAsync();
    }

    public async Task<Message> AddAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        return message;
    }
}