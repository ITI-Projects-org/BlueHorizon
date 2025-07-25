
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

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO _register)
        {
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


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO _login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(_login.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, _login.Password))
            {
                return Unauthorized("Invalid Email or Password");
            }

            var userData = new List<Claim>();
            userData.Add(new Claim("userId", user.Id)); 
            userData.Add(new Claim(ClaimTypes.NameIdentifier, user.Id)); 
            userData.Add(new Claim("username", user.UserName));
            userData.Add(new Claim("email", user.Email));

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                userData.Add(new Claim(ClaimTypes.Role, role));

            var key = "this is secrete key for admin role base";
            var secreteKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            var signingCredentials = new SigningCredentials(secreteKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken tokenObject = new JwtSecurityToken(
                claims: userData,
                expires: DateTime.Now.AddDays(10000), 
                signingCredentials: signingCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return Ok(new { token, roles = roles.ToList() });
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

        [HttpGet("Admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return Ok("Authorized Admin");
        }
    }
}