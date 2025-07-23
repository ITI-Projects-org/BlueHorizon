using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class QRCode
    {
        [Key]
        public int? Id { get; set; }
        [ForeignKey(nameof(Booking))]
        public int BookingId { get; set; }
        public string QRCodeValue { get; set; } //(the actual generated string/image data)
        public DateTime GeneratedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public virtual Booking Booking{ get; set; }
        public string TenantNationalId { get; set; }
        public string VillageName { get; set; }
        public string UnitAddress { get; set; }
        public string OwnerName { get; set; }
        public string TenantName { get; set; }
    }
}
