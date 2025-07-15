using System.ComponentModel.DataAnnotations;

namespace Village_System.Models
{
    public class Amenity
    {
        [Key]
        public int Id { get; set; }
        public AmenityName Name { get; set; }

    }
}
