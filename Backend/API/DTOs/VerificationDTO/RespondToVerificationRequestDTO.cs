using API.Models;

namespace API.DTOs.VerificationDTO
{
    public class RespondToVerificationRequestDTO
    {
        public string  OwnerId{ get; set; }
        public int UnitId{ get; set; }
        public VerificationStatus VerificationStatus{ get; set; }
    }
}
