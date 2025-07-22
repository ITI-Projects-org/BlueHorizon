using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations
{
    public class UnitRepository : GenericRepository<Unit>, IUnitRepository
    {
        public UnitRepository(BlueHorizonDbContext _context) : base(_context)
        {
        }
       

        public async Task<IEnumerable<Unit>> GetUnitsByOwnerIdAsync(string ownerId)
        {
            return await _context.Units
                .Where(u => u.OwnerId == ownerId)
                .Include(u => u.Owner).ToListAsync();

        }

        public async Task<Unit> GetUnitWithDetailsAsync(int id)
        {
            return await _context.Units
                .Include(u => u.Owner)
                .Include(u => u.UnitAmenities)
                .FirstOrDefaultAsync(u => u.Id == id);
        }


      


    }

}
   
    

