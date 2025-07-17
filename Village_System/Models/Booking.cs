using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Village_System.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Tenant))]
        public string TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }
        [ForeignKey(nameof(Unit))]
        public int UnitId { get; set; }
        public virtual Unit Unit { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PlatformComission { get; set; }
        public decimal OwnerPayoutAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public int NumberOfGuests { get; set; }
        public Boolean UnitReviewed { get; set; }
        public Boolean OwnerReviewd { get; set; }
        public virtual QRCode QRCode { get; set; }
    }
}

