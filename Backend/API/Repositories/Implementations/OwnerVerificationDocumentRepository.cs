using Microsoft.EntityFrameworkCore;
using API.DTOs.VerificationDTO;
using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class OwnerVerificationDocumentRepository : GenericRepository<OwnerVerificationDocument>, IOwnerVerificationDocumentRepository
    {

        public OwnerVerificationDocumentRepository(VillageSystemDbContext _context) : base(_context){}
        public async Task<IEnumerable<OwnerWithUnitVerificationDTO>> GetPendingOwnersWithUnitAsync()
        {

            var data = _context.Owners.Join(_context.OwnerVerificationDocuments,
                o => o.Id,
                ov => ov.OwnerId,
               (o, ov) =>
             new {
                 Owner = o,
                 OwnerVerification = ov
             }).Join(_context.Units,
             
             d=>d.Owner.Id,
             u=>u.OwnerId,
             (d, u) => new
             {
                 Owner = d.Owner,
                 OwnerVerification = d.OwnerVerification,
                 Unit = u});

            var result = data
                .Where(d=>d.Owner.VerificationStatus == VerificationStatus.Pending)
                .Select(o => new OwnerWithUnitVerificationDTO
            {
                OwnerId = o.Owner.Id,
                VerificationStatus = o.Owner.VerificationStatus,
                BankAccountDetails = o.Owner.BankAccountDetails,
                NationalId = o.OwnerVerification.NationalId,
                DocumentType = o.OwnerVerification.DocumentType,
                DocumentPath = o.OwnerVerification.DocumentPath,
                UploadDate = o.OwnerVerification.UploadDate,
                VerificationNotes = o.Owner.VerificationNotes,
                OwnerName = o.Owner.UserName,
                UnitId = o.Unit.Id,
                Address = o.Unit.Address,
                UnitType = o.Unit.UnitType,
                ContractPath = o.Unit.ContractPath,
                Contract = o.Unit.Contract
            }).ToList();
            return result;
        }
    }
    
}
