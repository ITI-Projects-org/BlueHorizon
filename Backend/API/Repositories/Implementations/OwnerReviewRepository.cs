using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class OwnerReviewRepository : GenericRepository<OwnerReview>, IOwnerReviewRepository
    {
        public OwnerReviewRepository(VillageSystemDbContext _context) : base(_context)
        {
        }
    }
    
}
