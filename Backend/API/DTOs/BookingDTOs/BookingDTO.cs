namespace API.DTOs.BookingDTOs
{
    public class BookingDTO
    {
        public int? Id { get; set; }
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
