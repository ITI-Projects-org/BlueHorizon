using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(BlueHorizonDbContext _context) : base(_context) { }
    }
}
