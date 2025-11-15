using Carma.Application.Abstractions;
using Carma.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Carma.Infrastructure;

public class CarmaDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, ICarmaDbContext
{
    public DbSet<Message> Messages { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Ride> Rides { get; set; }
    public DbSet<RideParticipant> RideParticipants { get; set; }
    public DbSet<Review> Reviews { get; set; }
    
    public CarmaDbContext(DbContextOptions<CarmaDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CarmaDbContext).Assembly);
    }
}