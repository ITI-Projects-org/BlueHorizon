using Village_System.Models;
using Village_System.Repositories.Interfaces;

namespace Village_System.Repositories.Implementations
{
    public class AccessPermissionRepository : GenericRepository<AccessPermission>, IAccessPermissionRepository
    {
        public AccessPermissionRepository(VillageSystemDbContext _context): base(_context)
        {
            
        }
    }
}
