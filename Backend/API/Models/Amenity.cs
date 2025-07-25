using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Amenity
    {
        [Key]
        public int Id { get; set; }
        public AmenityName Name { get; set; }
        public virtual ICollection<UnitAmenity>? UnitAmenities { get; set; }
    }
}
