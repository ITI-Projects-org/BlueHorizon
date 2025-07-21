using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class UnitImage
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Unit))]
        public int UnitId { get; set; }
        public string ImageURL { get; set; }
        public virtual Unit Unit{ get; set; }   
    }
}
