using Carma.Domain.Entities;

namespace Carma.Application.Abstractions;

public interface IRideParticipantRepository : IGenericRepository<RideParticipant>
{
    Task<IEnumerable<RideParticipant>> GetAllByRideIdAsync(int rideId);
    Task<RideParticipant?> GetByRideAndUserAsync(int rideId, Guid rideParticipantId);
}