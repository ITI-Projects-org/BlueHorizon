using API.Models;
using API.Repositories.Interfaces;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;


namespace API.Repositories.Implementations
{
    public class UnitRepository : GenericRepository<Unit>, IUnitRepository
    {
        public UnitRepository(BlueHorizonDbContext _context) : base(_context)
        {
        }


        public async Task<IEnumerable<Unit>> GetUnitsByOwnerIdAsync(string ownerId)
        {
            return await _context.Units
                .Where(u => u.OwnerId == ownerId)
                .Include(u => u.Owner).ToListAsync();

        }

        public async Task<Unit> GetUnitWithDetailsAsync(int id)
        {
            return await _context.Units
                .Include(u => u.Owner)
                .Include(u => u.UnitAmenities)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<IEnumerable<Unit>> GetAllValidUnits()
        {
            return await _context.Units
                .Where(u => u.VerificationStatus == VerificationStatus.Verified)
                .ToListAsync();
        }

        public async Task<string> GetSingleImagePathByUnitId(int unitId)
        {
            return await _context.UnitImages
                .Where(ui=>ui.UnitID == unitId)
                .Select(ui=>ui.ImageURL)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Unit>> GetAllPendingUnits()
        {
            return await _context.Units
                .Where(u => u.VerificationStatus == VerificationStatus.Pending)
                .Include(u => u.Owner)
                .ToListAsync();
        }




    }

}


