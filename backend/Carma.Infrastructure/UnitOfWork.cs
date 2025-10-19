using Carma.Application.Abstractions;

namespace Carma.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly CarmaDbContext context;

    public UnitOfWork(CarmaDbContext context)
    {
        this.context = context;
    }
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}