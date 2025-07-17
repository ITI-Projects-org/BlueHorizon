using Microsoft.EntityFrameworkCore;
using Village_System.DTOs.VerificationDTO;
using Village_System.Models;
using Village_System.Repositories.Interfaces;

namespace Village_System.Repositories.Implementations
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
             });


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
                //Unit = _context.Units.Where(u => u.OwnerId == o.Owner.Id).FirstOrDefault()
                UnitId = _context.Units.Where(u => u.OwnerId == o.Owner.Id).FirstOrDefault().Id,
                Address = _context.Units.Where(u => u.OwnerId == o.Owner.Id).FirstOrDefault().Address,
                UnitType = _context.Units.Where(u => u.OwnerId == o.Owner.Id).FirstOrDefault().UnitType,
                ContractPath = _context.Units.Where(u => u.OwnerId == o.Owner.Id).FirstOrDefault().ContractPath,
                Contract = _context.Units.Where(u => u.OwnerId == o.Owner.Id).FirstOrDefault().Contract

            }).ToList();
            return result;
        }
    }
    
}
