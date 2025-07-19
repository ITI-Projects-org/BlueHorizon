using System.Reflection.Metadata.Ecma335;
using API.DTOs.VerificationDTO;
using API.Models;
using API.Repositories.Interfaces;
   using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace API.Repositories.Implementations
{
    public class OwnerRepository : GenericRepository<Owner>, IOwnerRepository
    {
        public OwnerRepository(VillageSystemDbContext context) : base(context)
        {
            
        }
        
        
    }
}
