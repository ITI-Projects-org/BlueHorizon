using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class UnitReviewRepository : GenericRepository<UnitReview>, IUnitReviewRepository
    {
        public UnitReviewRepository(VillageSystemDbContext _context) : base(_context)
        {
        }

      
    }
    
}
