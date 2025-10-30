using Carma.Application.Abstractions;
using Carma.Application.Abstractions.Repositories;

namespace Carma.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly CarmaDbContext context;
    
    public IRideRepository Rides { get; }
    public IRideParticipantRepository RideParticipants { get; }
    public INotificationRepository Notifications { get; }
    public IMessageRepository Messages { get; }

    public UnitOfWork(CarmaDbContext context, IRideRepository rideRepository, IRideParticipantRepository rideParticipantRepository, INotificationRepository notifications, IMessageRepository messages)
    {
        this.context = context;
        Rides = rideRepository;
        RideParticipants = rideParticipantRepository;
        Notifications = notifications;
        Messages = messages;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}