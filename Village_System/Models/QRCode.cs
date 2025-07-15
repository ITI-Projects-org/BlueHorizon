using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Village_System.Models
{
    public class QRCode
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Booking))]
        public int BookingId { get; set; }
        public string QRCodeValue { get; set; } //(the actual generated string/image data)
        public DateTime GeneratedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
