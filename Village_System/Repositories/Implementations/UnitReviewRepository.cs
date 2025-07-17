using Village_System.Models;
using Village_System.Repositories.Interfaces;

namespace Village_System.Repositories.Implementations
{
    public class UnitReviewRepository : GenericRepository<UnitReview>, IUnitReviewRepository
    {
        public UnitReviewRepository(VillageSystemDbContext _context) : base(_context)
        {
        }

      
    }
    
}
