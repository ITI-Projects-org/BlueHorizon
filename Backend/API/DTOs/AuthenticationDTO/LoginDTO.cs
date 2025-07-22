using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace API.DTOs.AuthenticationDTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "First Name is required")]
        public string Email { get; set; }
        public string Username{ get; set; }
        public string Password{ get; set; }
        public string Role { get; set; }
    }
}
