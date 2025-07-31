namespace API.DTOs.BookingDTOs
{
    public class addBookingDTO
    {
        public int UnitId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
    }
}
