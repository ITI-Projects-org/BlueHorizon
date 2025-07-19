using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace API.Models
{
    public class Unit
    {
        [Key]
        public int Id { get; set; }
        public string OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public virtual Owner Owner { get; set; } // navigaiton prop
        public string Title { get; set; }
        public string Description { get; set; }
        public UnitType UnitType { get; set; }
        public int Bedrooms { get; set; }

        public int Bathrooms { get; set; }
        public int Sleeps { get; set; }
        public int DistanceToSea { get; set; }
        public decimal BasePricePerNight { get; set; }
        public string Address { get; set; }
        public string VillageName { get; set; }
        public DateTime CreationDate { get; set; }
        public float AverageUnitRating { get; set; }
        public virtual ICollection<UnitAmenity> UnitAmenities { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public DocumentType Contract{ get; set; }
        public string ContractPath { get; set; }
    }
}
