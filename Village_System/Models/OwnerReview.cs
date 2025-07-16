using System.ComponentModel.DataAnnotations.Schema;

namespace Village_System.Models
{
    public class OwnerReview
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Owner))]
        public string OwnerId { get; set; }
        public virtual Owner Owner { get; set; }
        [ForeignKey(nameof(Tenant))]
        public string TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }
        [ForeignKey(nameof(Booking))]
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public ReviewStatus ReviewStatus { get; set; }
    }
}
