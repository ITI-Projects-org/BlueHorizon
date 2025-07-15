using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;

namespace Village_System.Models
{
    public class UnitReview
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Unit))]
        public int UnitId { get; set; }
        [ForeignKey(nameof(Tenant))]
        public string TenantID { get; set; }
        [ForeignKey(nameof(Booking))]
        public int BookingId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public ReviewStatus ReviewStatus { get; set; }
    }
}

