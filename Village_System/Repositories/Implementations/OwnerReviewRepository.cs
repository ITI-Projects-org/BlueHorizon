using Village_System.Models;
using Village_System.Repositories.Interfaces;

namespace Village_System.Repositories.Implementations
{
    public class OwnerReviewRepository : GenericRepository<OwnerReview>, IOwnerReviewRepository
    {
        public OwnerReviewRepository(VillageSystemDbContext _context) : base(_context)
        {
        }
    }
    
}
