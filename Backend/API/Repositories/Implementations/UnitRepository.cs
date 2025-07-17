using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class UnitRepository : GenericRepository<Unit>, IUnitRepository
    {
        public UnitRepository(VillageSystemDbContext _context) : base(_context)
        {
        }
    }
   
    
}
