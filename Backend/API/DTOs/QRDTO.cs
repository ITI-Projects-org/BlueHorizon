namespace API.DTOs
{
    public class QRDTO
    {
        public int BookingId { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string TenantNationalId { get; set; }
        public string VillageName { get; set; }
        public string UnitAddress { get; set; }
        public string OwnerName { get; set; }
        public string TenantName { get; set; }
    }
}
