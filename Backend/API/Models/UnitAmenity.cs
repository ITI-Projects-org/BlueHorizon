using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    [PrimaryKey(nameof(UnitId), nameof(AmenityId))]
    public class UnitAmenity
    {
        [ForeignKey(nameof(Unit))]
        public int UnitId { get; set; }
        public virtual Unit? Unit{ get; set; }
        [ForeignKey(nameof(Amenity))]
        public int AmenityId { get; set; }
        public virtual Amenity Amenity { get; set; }

    }
}
