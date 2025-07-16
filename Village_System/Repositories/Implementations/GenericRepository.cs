using Microsoft.EntityFrameworkCore;
using Village_System.Models;
using Village_System.Repositories.Interfaces;

namespace Village_System.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public VillageSystemDbContext _context { get; }
        public GenericRepository(VillageSystemDbContext context)
        {
            _context = context;
        }
        public async  Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await _context.Set<T>().FindAsync(id);

        }

        public T UpdateByIdAsync(int id, T entity)
        {
            _context.Set<T>().Update(entity);
            return entity;
        }


        public T UpdateByIdAsync(string id, T entity)
        {
            _context.Set<T>().Update(entity);
            return entity;
        }
        public async void DeleteByIdAsync(int id)
        {
            _context.Set<T>().Remove(await GetByIdAsync(id));

        }

        public async void DeleteByIdAsync(string id)
        {
            _context.Set<T>().Remove(await GetByIdAsync(id));

        }

     
    }
}
