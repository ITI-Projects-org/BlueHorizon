using Village_System.Models;

namespace Village_System.DTOs.VerificationDTO
{
    public class RespondToVerificationRequestDTO
    {
        public string  OwnerId{ get; set; }
        public int UnitId{ get; set; }
        public VerificationStatus VerificationStatus{ get; set; }
    }
}
