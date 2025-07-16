using Village_System.DTOs.VerificationDTO;
using Village_System.Models;

namespace Village_System.Repositories.Interfaces
{
    public interface IOwnerVerificationDocumentRepository : IGenericRepository<OwnerVerificationDocument>
    {
        Task<IEnumerable<OwnerWithUnitVerificationDTO>> GetPendingOwnersWithUnitAsync();

    }
}
