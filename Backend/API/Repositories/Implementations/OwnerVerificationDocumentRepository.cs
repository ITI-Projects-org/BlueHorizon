using Microsoft.EntityFrameworkCore;
using API.DTOs.VerificationDTO;
using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class OwnerVerificationDocumentRepository : GenericRepository<OwnerVerificationDocument>, IOwnerVerificationDocumentRepository
    {

        public OwnerVerificationDocumentRepository(VillageSystemDbContext _context) : base(_context) { }

        //    public async Task<IEnumerable<OwnerWithUnitVerificationDTO>> GetPendingOwnersWithUnitAsync()
        //    {

        //        var data = await _context.Owners.Join(_context.OwnerVerificationDocuments,
        //            o => o.Id,
        //            ov => ov.OwnerId,
        //           (o, ov) =>
        //         new {
        //             Owner = o,
        //             OwnerVerification = ov
        //         }).Join(_context.Units,

        //         d=>d.Owner.Id,
        //         u=>u.OwnerId,
        //         (d, u) => new
        //         {
        //             Owner = d.Owner,
        //             OwnerVerification = d.OwnerVerification,
        //             Unit = u}).ToListAsync();

        //        var result = data
        //            .Where(d=>d.Owner.VerificationStatus == VerificationStatus.Pending)
        //            .Select(o => new OwnerWithUnitVerificationDTO
        //        {
        //            OwnerId = o.Owner.Id,
        //            VerificationStatus = o.Owner.VerificationStatus,
        //            BankAccountDetails = o.Owner.BankAccountDetails,
        //            NationalId = o.OwnerVerification.NationalId,
        //            DocumentType = o.OwnerVerification.DocumentType,
        //            DocumentPath = o.OwnerVerification.DocumentPath,
        //            UploadDate = o.OwnerVerification.UploadDate,
        //            VerificationNotes = o.Owner.VerificationNotes,
                //    OwnerName = o.Owner.UserName,
        //            UnitId = o.Unit.Id,
        //            Address = o.Unit.Address,
        //            UnitType = o.Unit.UnitType,
        //            ContractPath = o.Unit.ContractPath,
        //            Contract = o.Unit.Contract
        //        }).ToList();
        //        return result;
        //    }
        
        public async Task<IEnumerable<OwnerWithUnitVerificationDTO>> GetPendingOwnersWithUnitAsync()
        {
            
    var data = await _context.Owners
        .Where(o => o.VerificationStatus == VerificationStatus.Pending)
        .Select(o => new OwnerWithUnitVerificationDTO
        {
            OwnerId = o.Id,
            OwnerName = o.UserName,
            VerificationStatus = o.VerificationStatus,
            BankAccountDetails = o.BankAccountDetails ?? string.Empty,
            VerificationNotes = o.VerificationNotes,
            
            // Get the first verification document using a subquery with null handling
            NationalId = _context.OwnerVerificationDocuments
                .Where(doc => doc.OwnerId == o.Id)
                .Select(doc => doc.NationalId)
                .FirstOrDefault() ?? string.Empty,
            DocumentType = _context.OwnerVerificationDocuments
                .Where(doc => doc.OwnerId == o.Id)
                .Select(doc => doc.DocumentType)
                .FirstOrDefault(),
            FrontNationalIdDocumentPath = _context.OwnerVerificationDocuments
                .Where(doc => doc.OwnerId == o.Id)
                .Select(doc => doc.FrontNationalIdDocumentPath)
                .FirstOrDefault() ?? string.Empty,
            BackNationalIdDocumentPath = _context.OwnerVerificationDocuments
                .Where(doc => doc.OwnerId == o.Id)
                .Select(doc => doc.BackNationalIdDocumentPath)
                .FirstOrDefault() ?? string.Empty,
            UploadDate = _context.OwnerVerificationDocuments
                .Where(doc => doc.OwnerId == o.Id)
                .Select(doc => doc.UploadDate)
                .FirstOrDefault(),
            
            // Get the first unit using a subquery with null handling
            UnitId = _context.Units
                .Where(u => u.OwnerId == o.Id)
                .Select(u => u.Id)
                .FirstOrDefault(),
            Title = _context.Units
                .Where(u => u.OwnerId == o.Id)
                .Select(u => u.Title)
                .FirstOrDefault() ?? string.Empty,
            Description = _context.Units
                .Where(u => u.OwnerId == o.Id)
                .Select(u => u.Description)
                .FirstOrDefault() ?? string.Empty,
            Address = _context.Units
                .Where(u => u.OwnerId == o.Id)
                .Select(u => u.Address)
                .FirstOrDefault() ?? string.Empty,
            UnitType = _context.Units
                .Where(u => u.OwnerId == o.Id)
                .Select(u => u.UnitType)
                .FirstOrDefault(),
            Bedrooms = _context.Units
                .Where(u => u.OwnerId == o.Id)
                .Select(u => u.Bedrooms)
                .FirstOrDefault(),
            Bathrooms = _context.Units
                .Where(u => u.OwnerId == o.Id)
                .Select(u => u.Bathrooms)
                .FirstOrDefault(),
            Sleeps = _context.Units
                .Where(u => u.OwnerId == o.Id)
                .Select(u => u.Sleeps)
                .FirstOrDefault(),
            DistanceToSea = _context.Units
                .Where(u => u.OwnerId == o.Id)
                .Select(u => u.DistanceToSea)
                .FirstOrDefault(),
            BasePricePerNight = _context.Units
                .Where(u => u.OwnerId == o.Id)
                .Select(u => u.BasePricePerNight)
                .FirstOrDefault(),
            VillageName = _context.Units
                .Where(u => u.OwnerId == o.Id)
                .Select(u => u.VillageName)
                .FirstOrDefault() ?? string.Empty,
            CreationDate = _context.Units
                .Where(u => u.OwnerId == o.Id)
                .Select(u => u.CreationDate)
                .FirstOrDefault(),
            AverageUnitRating = _context.Units
                .Where(u => u.OwnerId == o.Id)
                .Select(u => u.AverageUnitRating)
                .FirstOrDefault(),
            ContractPath = _context.Units
                .Where(u => u.OwnerId == o.Id)
                .Select(u => u.ContractPath)
                .FirstOrDefault() ?? string.Empty,
            Contract = _context.Units
                .Where(u => u.OwnerId == o.Id)
                .Select(u => u.Contract)
                .FirstOrDefault()
        })
        .ToListAsync();
            return data;
        }
    }   
}
