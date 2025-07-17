using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace Village_System.DTOs.AuthenticationDTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "First Name is required")]
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        [Compare("Password",ErrorMessage="Confirm Password doesn't match Password")]
        public string ConfirmPassword{ get; set; }
        public string Role { get; set; }
        
    }
}
