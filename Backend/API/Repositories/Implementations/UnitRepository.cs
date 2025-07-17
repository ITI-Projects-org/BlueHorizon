using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class UnitRepository : GenericRepository<Unit>, IUnitRepository
    {
        public UnitRepository(BlueHorizonDbContext _context) : base(_context)
        {
        }
    }
   
    
}
