using Carma.Application.Abstractions.Repositories;
using Carma.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Carma.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly CarmaDbContext _context;

    public ReviewRepository(CarmaDbContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<Review>> GetAllForUserAsync(Guid userId)
    {
        var reviews = await _context.Reviews
            .Include(r => r.Reviewer)
            .Where(r => r.ReviewedUserId == userId)
            .ToListAsync();
        
        return reviews;
    }

    public async Task<Review> AddAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
        return review;
    }
}