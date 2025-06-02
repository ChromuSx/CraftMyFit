using Microsoft.EntityFrameworkCore;

namespace CraftMyFit.Data.Repositories.Base
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly CraftMyFitDbContext _context;

        public Repository(CraftMyFitDbContext context) => _context = context;

        public virtual async Task<List<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public virtual async Task<T> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);

        public virtual async Task<int> AddAsync(T entity)
        {
            _ = await _context.Set<T>().AddAsync(entity);
            _ = await _context.SaveChangesAsync();

            // Assumiamo che l'entità abbia una proprietà Id
            System.Reflection.PropertyInfo? idProperty = entity.GetType().GetProperty("Id");
            return (int)idProperty.GetValue(entity);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _ = _context.Set<T>().Update(entity);
            _ = await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            T entity = await GetByIdAsync(id);
            if(entity != null)
            {
                _ = _context.Set<T>().Remove(entity);
                _ = await _context.SaveChangesAsync();
            }
        }
    }
}
