using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class OwnerReviewRepository : GenericRepository<OwnerReview>, IOwnerReviewRepository
    {
        public OwnerReviewRepository(BlueHorizonDbContext _context) : base(_context)
        {
        }
    }
    
}
