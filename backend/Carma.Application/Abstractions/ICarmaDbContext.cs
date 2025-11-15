using Carma.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Carma.Application.Abstractions;

public interface ICarmaDbContext
{
    DbSet<User> Users { get; }
    DbSet<Message> Messages { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<Ride> Rides { get; }
    DbSet<RideParticipant> RideParticipants { get; }
    DbSet<Review> Reviews { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}