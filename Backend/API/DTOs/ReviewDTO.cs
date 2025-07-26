using API.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTOs
{
    public class ReviewDTO
    {
        public int UnitId { get; set; }
        public string? TenantId { get; set; }
        public string? TenantName { get; set; }
        public int BookingId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? ReviewDate { get; set; }
        public ReviewStatus? ReviewStatus { get; set; }

    }
}
