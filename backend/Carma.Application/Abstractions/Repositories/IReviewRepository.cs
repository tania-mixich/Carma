using Carma.Domain.Entities;

namespace Carma.Application.Abstractions.Repositories;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetAllForUserAsync(Guid userId);
    Task<Review> AddAsync(Review review);
}