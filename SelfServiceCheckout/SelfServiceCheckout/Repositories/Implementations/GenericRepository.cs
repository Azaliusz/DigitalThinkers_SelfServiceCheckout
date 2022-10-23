using Microsoft.EntityFrameworkCore;
using SelfServiceCheckout.Repositories.Abstractions;

namespace SelfServiceCheckout.Repositories.Implementations
{
    public class GenericRepository<T, TDbContext> : IGenericRepository<T, TDbContext>
        where T : class
        where TDbContext : DbContext
    {
        protected readonly TDbContext _context;

        public GenericRepository(TDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<T?> GetAsync(params object?[]? keyValues)
        {
            return keyValues != null && keyValues.Length != 0
                ? await _context.Set<T>().FindAsync(keyValues)
                : null;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(params object?[]? keyValues)
        {
            var entity = await GetAsync(keyValues);
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exists(params object?[]? keyValues)
        {
            var entity = await GetAsync(keyValues);
            return entity != null;
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}