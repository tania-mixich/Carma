using Carma.Domain.Entities;

namespace Carma.Application.Abstractions;

public interface IRideRepository : IGenericRepository<Ride>
{
    Task<IEnumerable<Ride>> GetAllAsync();
}