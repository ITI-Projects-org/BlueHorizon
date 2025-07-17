using Microsoft.EntityFrameworkCore;
using Village_System.DTOs.UnitDTO;
using Village_System.Models;
using Village_System.Repositories.Interfaces;

namespace Village_System.Repositories.Implementations
{
    public class UnitRepository : GenericRepository<Unit>, IUnitRepository
    {
        private readonly VillageSystemDbContext context;

        public UnitRepository(VillageSystemDbContext _context) : base(_context)
        {
            this.context = _context;
        }

        public async Task<Unit> GetByIdAsync(int id)
        {
            return await context.Units
                .Include(u => u.Owner)
                .FirstOrDefaultAsync(u => u.Id == id);

        }
        public async Task<Unit> UpdateByIdAsync(int id,Unit unit)
        {
            if (unit.Id != id)
            {
                throw new ArgumentException("Unit ID mismatch");
            }
            context.Entry(unit).State = EntityState.Modified;

            return unit;
        }
        
    }
   
    
}
