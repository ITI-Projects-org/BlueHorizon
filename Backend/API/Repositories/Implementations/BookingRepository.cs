using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(VillageSystemDbContext _context) : base(_context) { }
    }
}
