﻿using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class AmenityRepository : GenericRepository<Amenity> , IAmenityRepository
    {
        public AmenityRepository(BlueHorizonDbContext _context) : base(_context){ }
    }
}
