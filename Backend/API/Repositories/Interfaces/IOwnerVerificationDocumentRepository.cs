using API.DTOs.VerificationDTO;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IOwnerVerificationDocumentRepository : IGenericRepository<OwnerVerificationDocument>
    {
        Task<IEnumerable<OwnerWithUnitVerificationDTO>> GetPendingOwnersWithUnitAsync();

    }
}
