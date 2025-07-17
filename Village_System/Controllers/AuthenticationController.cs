using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Village_System.DTOs.AuthenticationDTO;
using Village_System.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
namespace Village_System.Controllers
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
        [HttpPost("Register")]

        public async Task<IActionResult> Register([FromBody] RegisterDTO _register)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not Valid Form Data!");
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
                return Ok(new { Message = "registerd" });
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            };

            return BadRequest("Cannot Register");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO _login)
        {
            if (!ModelState.IsValid) return BadRequest("Not Valid Form Data!");
            var user = await _userManager.FindByEmailAsync(_login.Email);
            if (user == null) return Unauthorized("This Email Doesn't Exists ");
            var IsCorrectPassword = await _userManager.CheckPasswordAsync(user, _login.Password);
            if (!IsCorrectPassword) return Unauthorized("Wrong Email or Password");

            #region Claims
            var userData = new List<Claim>();
            userData.Add(new Claim("userId", user.Id));
            userData.Add(new Claim("username", _login.Username));
            userData.Add(new Claim("email", user.Email));

            // Get user type from discriminator instead of GetType() to avoid proxy issues
            var userType = user is Owner ? "Owner" : user is Tenant ? "Tenant" : "Admin";
            userData.Add(new Claim(ClaimTypes.Role, userType));

            var roles = await _userManager.GetRolesAsync(user);
            Console.WriteLine($"User {user.Email} has roles: {string.Join(", ", roles)}"); // Debug line

            foreach (var role in roles)
                userData.Add(new Claim(ClaimTypes.Role, role));

            // If no roles found, add a default role or log it
            if (!roles.Any())
                Console.WriteLine($"Warning: User {user.Email} has no roles assigned!");


            #endregion
            #region SigningCredentials
            var key = "this is secrete key  for admin role base";
            var secreteKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            var signingCredentials = new SigningCredentials(secreteKey, SecurityAlgorithms.HmacSha256);
            #endregion

            JwtSecurityToken tokenObject = new JwtSecurityToken(
                claims: userData,
                expires: DateTime.Now.AddDays(10000),
                signingCredentials: signingCredentials
                );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return Ok(new { token, roles = roles.ToList() }); // Return roles in response for debugging

        }


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
    }

}
