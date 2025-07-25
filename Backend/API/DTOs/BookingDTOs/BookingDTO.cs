using API.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTOs.BookingDTOs
{
    public class BookingDTO
    {
        public string? TenantId { get; set; }
        public int UnitId { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? PlatformComission { get; set; }
        public decimal? OwnerPayoutAmount { get; set; }
        //public PaymentStatus PaymentStatus { get; set; }
        public int NumberOfGuests { get; set; }
    }
}
