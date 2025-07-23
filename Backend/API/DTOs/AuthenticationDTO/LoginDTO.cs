using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

//namespace API.DTOs.AuthenticationDTO
//{
//    public class LoginDTO
//    {
//        //[Required(ErrorMessage = "First Name is required")]
//        public string Email { get; set; }
//        //public string Username{ get; set; }
//        public string Password { get; set; }
//        //public string Role { get; set; }
//    }
//}
namespace API.DTOs.AuthenticationDTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required")] // 👈 شيلت التعليق هنا
        [EmailAddress(ErrorMessage = "Invalid email format")] // 👈 أضفت السطر ده
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")] // 👈 شيلت التعليق هنا
        public string Password { get; set; }
        // تم إزالة تعليق الخصائص الأخرى (Username, Role) تمامًا لأننا لا نحتاجها هنا.
    }
}
