using Village_System.Models;
using Village_System.Repositories.Interfaces;

namespace Village_System.Repositories.Implementations
{
    public class OwnerVerificationDocumentRepository : GenericRepository<OwnerVerificationDocument>, IOwnerVerificationDocumentRepository
    {
        public OwnerVerificationDocumentRepository(VillageSystemDbContext _context) : base(_context)
        {
        }
    }
    
}
