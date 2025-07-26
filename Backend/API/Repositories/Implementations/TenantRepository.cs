using System.Reflection.Metadata.Ecma335;
using API.DTOs.VerificationDTO;
using API.Models;
using API.Repositories.Interfaces;
   using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Org.BouncyCastle.Asn1.TeleTrust;

namespace API.Repositories.Implementations
{
    public class TenantRepository : GenericRepository<Tenant>, ITenantRepository
    {
        public TenantRepository(BlueHorizonDbContext context) : base(context)
        {
            
        }
        public async Task<string> GetTenantNameBuUserId(string userId)
        {
            var tenant = await _context.Tenants.Where(t => t.Id == userId).FirstOrDefaultAsync();
            return tenant.UserName ?? "unknown";
        }
    }
}
