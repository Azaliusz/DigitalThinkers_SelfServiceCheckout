using Microsoft.EntityFrameworkCore;

namespace SelfServiceCheckout.Repositories.Abstractions
{
    public interface IGenericRepository<T, TDbContext>
        where T : class
        where TDbContext : DbContext
    {
        Task<T?> GetAsync(params object?[]? keyValues);

        Task<IEnumerable<T>> GetAllAsync();

        Task<T> AddAsync(T entity);

        Task DeleteAsync(params object?[]? keyValues);

        Task UpdateAsync(T entity);

        Task<bool> Exists(params object?[]? keyValues);
    }
}