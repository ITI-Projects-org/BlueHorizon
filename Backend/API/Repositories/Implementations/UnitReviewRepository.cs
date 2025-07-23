using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class UnitReviewRepository : GenericRepository<UnitReview>, IUnitReviewRepository
    {
        public UnitReviewRepository(BlueHorizonDbContext _context) : base(_context)
        {
        }


        double IUnitReviewRepository.CalculateAverageRating(int unitId)
        {
            var x = _context.UnitReviews
                .Where(u => u.UnitId == unitId)
                .ToList()
                .Average(u => u.Rating);
            return x;

        }

        async Task UpdateAverageRating(int unitId)
        {

            //    var newRating = IUnitRepository.CalculateAverageRating(unitId);
            //    Unit unit= await GetByIdAsync(unitId);
            //    if (unit != null) { return ; }

            //    _context.Units.Update(await GetByIdAsync(unitId));
        }

        Task IUnitReviewRepository.UpdateAverageRating(int unitId)
        {
            throw new NotImplementedException();
        }
    }
    
}
