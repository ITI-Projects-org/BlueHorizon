using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations
{
    public class UnitImagesRepository : GenericRepository<UnitImages>, IUnitImagesRepository
    {
        private readonly BlueHorizonDbContext _context;
        public UnitImagesRepository(BlueHorizonDbContext context) : base(context)
        {
            _context = context;

        }

        public void DeleteAllImagesByUnitIdAsync(int unitId)
        {
            _context.UnitImages
                .Where(ui => ui.UnitID == unitId)
                .ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<UnitImages>> GetUnitImagesByUnitIdAsync(int unitId)
        {

            return await _context.UnitImages
                .Where(ui => ui.UnitID == unitId)
                .ToListAsync();
        }
    }
}
