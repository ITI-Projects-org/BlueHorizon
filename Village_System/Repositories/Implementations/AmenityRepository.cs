using Village_System.Models;
using Village_System.Repositories.Interfaces;

namespace Village_System.Repositories.Implementations
{
    public class AmenityRepository : GenericRepository<Amenity> , IAmenityRepository
    {
        public AmenityRepository(VillageSystemDbContext _context) : base(_context){ }
    }
}
