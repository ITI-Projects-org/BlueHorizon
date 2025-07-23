using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations
{
    public class QRCodeRepository : GenericRepository<QRCode>, IQRCodeRepository
    {
        public QRCodeRepository(VillageSystemDbContext _context) : base(_context){ }

        public Task<QRCode> GetQrCodeByBookingId(int BookingId)
        {
            return _context.QRCodes.FirstOrDefaultAsync(q => q.BookingId == BookingId);
        }
    }
}
