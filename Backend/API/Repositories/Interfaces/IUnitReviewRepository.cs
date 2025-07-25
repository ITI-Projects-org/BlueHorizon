using API.DTOs;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IUnitReviewRepository : IGenericRepository<UnitReview>
    {
        double CalculateAverageRating(int unitId);
        Task<IEnumerable<UnitReview>> GetAllUnitReviews(int UnitId);
    }
}
