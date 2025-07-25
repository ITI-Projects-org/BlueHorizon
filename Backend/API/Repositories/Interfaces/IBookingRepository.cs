using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking> 
    {
        Task<Boolean> IsValidBooking(Unit unit, DateTime checkIn, DateTime checkOut);
    }
}
