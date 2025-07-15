using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

namespace Village_System.Models
{
    [PrimaryKey(nameof(UnitId), nameof(AmenityId))]
    public class UnitAmenity
    {
        [ForeignKey(nameof(Unit))]
        public int UnitId { get; set; }
        [ForeignKey(nameof(Amenity))]
        public int AmenityId { get; set; }
    }
}
