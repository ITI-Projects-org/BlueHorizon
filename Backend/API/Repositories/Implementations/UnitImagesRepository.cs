using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class UnitImagesRepository : GenericRepository<UnitImages>, IUnitImagesRepository
    {
        public UnitImagesRepository(BlueHorizonDbContext _context) : base(_context) { }

    }
}
