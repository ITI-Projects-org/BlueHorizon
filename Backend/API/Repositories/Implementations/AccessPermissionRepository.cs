using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class AccessPermissionRepository : GenericRepository<AccessPermission>, IAccessPermissionRepository
    {
        public AccessPermissionRepository(VillageSystemDbContext _context): base(_context)
        {
            
        }
    }
}
