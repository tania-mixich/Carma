using Carma.Application.Abstractions.Repositories;

namespace Carma.Application.Abstractions;

public interface IUnitOfWork
{
    IRideRepository Rides { get; }
    IRideParticipantRepository RideParticipants { get; }
    INotificationRepository Notifications { get; }
    IMessageRepository Messages { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}