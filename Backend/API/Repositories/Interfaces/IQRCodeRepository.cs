using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IQRCodeRepository : IGenericRepository<QRCode>
    {
        //Task<IEnumerable<QRCode>> GetAllTenantQrCodes(string tenantId);
        Task<QRCode> GetQrCodeByBookingId(int BookinId);
    }
}
