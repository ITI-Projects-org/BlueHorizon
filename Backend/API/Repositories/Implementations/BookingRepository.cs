using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(BlueHorizonDbContext _context) : base(_context) { }

        public async Task<Boolean> IsValidBooking(Unit unit, DateTime checkIn, DateTime checkOut)
        {
            foreach (var booking in unit.Bookings)
            {
                if (checkIn < booking.CheckOutDate && booking.CheckInDate < checkOut)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
