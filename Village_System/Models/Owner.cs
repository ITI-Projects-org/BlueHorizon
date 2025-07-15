
using System.ComponentModel.DataAnnotations.Schema;

namespace Village_System.Models
{
    [NotMapped]
    public class Owner : ApplicationUser
    {
        public string BankAccountDetails { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public DateTime VerificationDate { get; set; }
        public string VerificationNotes { get; set; }
        [NotMapped]
        public float AverageOwnerRating { get; set; } // calculated field
    }
}
