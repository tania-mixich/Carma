using Carma.Application.Abstractions;
using Carma.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Carma.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly CarmaDbContext _context;
    private readonly DbSet<T> _dbSet;
    
    public GenericRepository(CarmaDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}