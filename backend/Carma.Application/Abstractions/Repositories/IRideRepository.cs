using Carma.Domain.Entities;

namespace Carma.Application.Abstractions.Repositories;

public interface IRideRepository 
{
    Task<Ride> AddAsync(Ride ride);
    Task<Ride?> GetByIdAsync(int id);
    void Update(Ride ride);
    void Delete(Ride ride);
    Task<IEnumerable<Ride>> GetAllAsync();
}