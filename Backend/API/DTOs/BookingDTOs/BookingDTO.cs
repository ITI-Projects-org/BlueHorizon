using API.Models;
using System.ComponentModel.DataAnnotations.Schema;

using API.DTOs.UnitDTO;

namespace API.DTOs.BookingDTOs
{
    public class BookingDTO
    {
        //from booking
        public int Id { get; set; }
        public int UnitId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public string TenantId { get; set; }
        public DateTime BookingDate { get; set; }


        public decimal TotalPrice { get; set; }
        public decimal OwnerPayoutAmount { get; set; }
        public decimal PlatformComission { get; set; }
        public string PaymentStatus { get; set; }
        public bool unitReviewed { get; set; }
        public UnitDetailsDTO Unit{ get; set; }



    }
}
