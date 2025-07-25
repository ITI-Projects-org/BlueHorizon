using API.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTOs.BookingDTOs
{
    public class BookingDTO
    {
        public int UnitId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        //public PaymentStatus PaymentStatus { get; set; }
        public int NumberOfGuests { get; set; }
    }
}
