using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class AccessPermissionRepository : GenericRepository<AccessPermission>, IAccessPermissionRepository
    {
        public AccessPermissionRepository(BlueHorizonDbContext _context): base(_context)
        {
            
        }
    }
}
