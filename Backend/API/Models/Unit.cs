using MimeKit.Encodings;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Unit
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public string OwnerId { get; set; }
        public virtual Owner Owner { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public UnitType UnitType { get; set; } // Apartment, Chalet, Villa,
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Sleeps { get; set; }
        public int DistanceToSea { get; set; }
        public decimal BasePricePerNight { get; set; }
        public string Address { get; set; }
        public string VillageName { get; set; }
        public DateTime CreationDate { get; set; }
        public float AverageUnitRating { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<UnitAmenity> UnitAmenities { get; set; }
        public VerificationStatus VerificationStatus { get; set; } // NotVerified=0 ,Pending=1, Verified=2, Rejected=3
        public DocumentType Contract { get; set; } // OwnershipContract, NationaId_Front, NationaId_Back 
        public string ContractPath { get; set; }
        public virtual ICollection<UnitImages> UnitImagesTable { get; set; }
    }
}
