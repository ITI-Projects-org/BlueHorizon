using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IUnitRepository : IGenericRepository<Unit>
    {
        Task<IEnumerable<Unit>> GetUnitsByOwnerIdAsync(string ownerId);
        Task<Unit> GetUnitWithDetailsAsync(int id);
    }
}
