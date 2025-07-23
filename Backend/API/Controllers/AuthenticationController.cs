// API/Controllers/AuthenticationController.cs

// تأكد إن الـ using statements دي كلها موجودة في بداية الملف:
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using API.DTOs.AuthenticationDTO;
using API.Models;
using AutoMapper;
// ... (أي using آخر موجود عندك)

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        public UserManager<ApplicationUser> _userManager { get; }
        public IMapper _mapper { get; set; }

        public AuthenticationController(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        // دالة Register هتفضل زي ما هي، مش هنعدلها هنا
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO _register)
        {
            // ... (باقي كود دالة Register)
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(_register.Email);
            if (user != null) return BadRequest("This Email Exists before");

            try
            {
                if (_register.Role == "Tenant")
                {
                    Tenant tenant = _mapper.Map<Tenant>(_register);
                    var result = await _userManager.CreateAsync(tenant, _register.Password);
                    if (!result.Succeeded)
                        return BadRequest(result.Errors.Select(e => e.Description));
                    await _userManager.AddToRoleAsync(tenant, _register.Role);
                }
                else if (_register.Role == "Owner")
                {
                    Owner owner = _mapper.Map<Owner>(_register);
                    var result = await _userManager.CreateAsync(owner, _register.Password);
                    if (!result.Succeeded)
                        return BadRequest(result.Errors.Select(e => e.Description));
                    await _userManager.AddToRoleAsync(owner, _register.Role);
                }
                else if (_register.Role == "Admin")
                {
                    Admin admin = _mapper.Map<Admin>(_register);
                    var result = await _userManager.CreateAsync(admin, _register.Password);
                    if (!result.Succeeded)
                        return BadRequest(result.Errors.Select(e => e.Description));
                    await _userManager.AddToRoleAsync(admin, _register.Role);
                }
                else
                    return BadRequest("Invalid Role");

                return Ok(new { Message = "Registered successfully!" });
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
                return StatusCode(500, "An error occurred during registration.");
            }
        }


        // ✅ هذا هو الجزء الذي ستقوم بتعديله
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO _login)
        {
            // 1. تحقق من الـ validation بناءً على الـ LoginDTO الجديد
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // هيرجع رسائل الأخطاء اللي كتبتها في LoginDTO (مثل "Email is required")

            // 2. ابحث عن المستخدم بالإيميل اللي جاي في الـ DTO
            var user = await _userManager.FindByEmailAsync(_login.Email);

            // 3. تحقق من وجود المستخدم وصحة كلمة المرور
            if (user == null || !await _userManager.CheckPasswordAsync(user, _login.Password))
            {
                // رسالة عامة لزيادة الأمان (لتجنب معرفة إذا كان الإيميل موجود أو كلمة المرور غلط)
                return Unauthorized("Invalid Email or Password");
            }

            // 4. بناء الـ Claims:
            var userData = new List<Claim>();
            userData.Add(new Claim("userId", user.Id));
            userData.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            // ✅ هنا بنضيف اسم المستخدم (UserName) من كائن الـ user اللي لقيناه
            userData.Add(new Claim("username", user.UserName));
            userData.Add(new Claim("email", user.Email));

            // 5. جلب الأدوار للمستخدم
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                userData.Add(new Claim(ClaimTypes.Role, role));

            // 6. إنشاء الـ SigningCredentials (المفتاح السري)
            // ملاحظة: تأكد إن هذا المفتاح قوي وطويل في البيئة الإنتاجية
            var key = "this is secrete key for admin role base";
            var secreteKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            var signingCredentials = new SigningCredentials(secreteKey, SecurityAlgorithms.HmacSha256);

            // 7. بناء الـ JWT Token
            JwtSecurityToken tokenObject = new JwtSecurityToken(
                claims: userData,
                expires: DateTime.Now.AddDays(10000), // يفضل تخليها مدة أقصر في الإنتاج (مثلاً: 7 أيام)
                signingCredentials: signingCredentials
            );

            // 8. كتابة الـ Token وإرجاع الرد
            var token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return Ok(new { token, roles = roles.ToList() });
        }

        // باقي الدوال (Tenant, Owner, Admin) هتفضل زي ما هي
        [HttpGet("Tenant")]
        [Authorize(Roles = "Tenant")]
        public IActionResult Tenant()
        {
            return Ok("Authorized Tenant");
        }

        [HttpGet("Owner")]
        [Authorize(Roles = "Owner")]
        public IActionResult Owner()
        {
            return Ok("Authorized Owner");
        }

        [HttpGet("Admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return Ok("Authorized Admin");
        }
    }
}