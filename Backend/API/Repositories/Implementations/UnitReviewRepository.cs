using System.Threading.Tasks;
using API.DTOs;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations
{
    public class UnitReviewRepository : GenericRepository<UnitReview>, IUnitReviewRepository
    {
        public UnitReviewRepository(VillageSystemDbContext _context) : base(_context)
        {
        }

        public async Task<IEnumerable<UnitReview>> GetAllUnitReviews(int UnitId)
        {
            return await _context.UnitReviews
            .Where(ur => ur.UnitId == UnitId).ToListAsync();
        }

        double IUnitReviewRepository.CalculateAverageRating(int unitId)
        {
            var x = _context.UnitReviews
                .Where(u => u.UnitId == unitId)
                .ToList()
                .Average(u => u.Rating);
            return x;

        }


    }

}
