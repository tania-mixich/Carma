using Carma.Domain.Entities;

namespace Carma.Application.Abstractions.Repositories;

public interface IRideParticipantRepository
{
    Task<RideParticipant> AddAsync(RideParticipant rideParticipant);
    Task<RideParticipant?> GetByRideAndUserAsync(int rideId, Guid rideParticipantId);
    void Update(RideParticipant rideParticipant);
    void Delete(RideParticipant rideParticipant);
    Task<bool> ContainsUserAsync(int rideId, Guid userId);
}