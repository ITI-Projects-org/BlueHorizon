using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class QRCodeRepository : GenericRepository<QRCode>, IQRCodeRepository
    {
        public QRCodeRepository(BlueHorizonDbContext _context) : base(_context)
        { }
    }
}
