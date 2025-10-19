namespace Carma.Application.Abstractions;

public interface IGenericRepository<T> 
{
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}