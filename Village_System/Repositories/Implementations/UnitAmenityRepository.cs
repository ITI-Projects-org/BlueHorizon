using Village_System.Models;
using Village_System.Repositories.Interfaces;

namespace Village_System.Repositories.Implementations
{
    public class UnitAmenityRepository : GenericRepository<UnitAmenity>, IUnitAmenityRepository
    {
        public UnitAmenityRepository(VillageSystemDbContext _context) : base(_context)
        { }
    }
}
