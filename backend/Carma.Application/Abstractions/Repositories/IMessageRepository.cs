using Carma.Domain.Entities;

namespace Carma.Application.Abstractions.Repositories;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetAllFromRideAsync(int rideId);
    Task<Message> AddAsync(Message message);
}