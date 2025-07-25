using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Identity.Client;

namespace API.Models
{
    public class UnitReview
    {
            public int Id { get; set; }
            [ForeignKey(nameof(Unit))]
            public int UnitId { get; set; }
            public virtual Unit Unit{ get; set; }
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

