using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace API.DTOs.AuthenticationDTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        [Compare("Password",ErrorMessage="Confirm Password doesn't match Password")]
        public string ConfirmPassword{ get; set; }
        public string Role { get; set; }
        
    }
}
