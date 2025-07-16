using System.Reflection.Metadata.Ecma335;
using Village_System.DTOs.VerificationDTO;
using Village_System.Models;
using Village_System.Repositories.Interfaces;
   using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Village_System.Repositories.Implementations
{
    public class OwnerRepository : GenericRepository<Owner>, IOwnerRepository
    {
        public OwnerRepository(VillageSystemDbContext context) : base(context)
        {
            
        }
        
        
    }
}
