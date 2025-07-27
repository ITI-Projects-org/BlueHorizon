using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IUnitImagesRepository : IGenericRepository<UnitImages>
    {
        Task<List<string>> GetImagesByUnitId(int unitId);
    }
}
