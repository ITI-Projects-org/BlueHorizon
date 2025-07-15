using System.ComponentModel.DataAnnotations.Schema;

namespace Village_System.Models
{
    public class OwnerReview
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Owner))]
        public int OwnerID { get; set; }
        [ForeignKey(nameof(Tenant))]
        public int TenantID{ get; set; }
        [ForeignKey(nameof(Booking))]
        public int BookingID  { get; set; }
        public int Rating { get; set; }
        public string Comment{ get; set; }
        public DateTime ReviewDate{ get; set; }
        public ReviewStatus ReviewStatus{ get; set; }
    }
}
