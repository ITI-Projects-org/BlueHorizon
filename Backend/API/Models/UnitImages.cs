
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class UnitImages
    {
        [Key]
        public int Id { get; set; }
        public string ImageURL { get; set; }

        [ForeignKey(nameof(Unit))]
        public int  UnitID { get; set; }

        public virtual Unit? Unit { get; set; }
    }
}


