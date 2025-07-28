using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations
{
    public class UnitImagesRepository : GenericRepository<UnitImages>, IUnitImagesRepository
    {
        public UnitImagesRepository(BlueHorizonDbContext _context) : base(_context) { }
        public async Task<List<string>> GetImagesByUnitId(int unitId)
        {
            var x = await _context.UnitImages
                .Where(ui => ui.UnitID == unitId)
                .Select(ui => ui.ImageURL)
                .ToListAsync();
            return x;

        }
    }
}
