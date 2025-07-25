﻿using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class UnitAmenityRepository : GenericRepository<UnitAmenity>, IUnitAmenityRepository
    {
        public UnitAmenityRepository(BlueHorizonDbContext _context) : base(_context)
        { }
    }
}
