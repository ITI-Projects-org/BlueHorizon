using Village_System.Models;
using Village_System.Repositories.Interfaces;

namespace Village_System.Repositories.Implementations
{
    public class QRCodeRepository : GenericRepository<QRCode>, IQRCodeRepository
    {
        public QRCodeRepository(VillageSystemDbContext _context) : base(_context)
        { }
    }
}
