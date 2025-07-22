using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using API.DTOs.AuthenticationDTO;
using API.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity.Data;
using API.Services.Interfaces;
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        public UserManager<ApplicationUser> _userManager { get; }
        public IMapper _mapper { get; set; }
        public IConfiguration _config { get; set; }
        public IAuthService _authService { get; set; }
        public AuthenticationController(UserManager<ApplicationUser> userManager, IMapper mapper, IConfiguration config, IAuthService authService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _config = config;
            _authService = authService;
        }


        [HttpGet("google-signup/{role}")]
        public IActionResult GoogleSignup(string role)
        {
            var redirectUrl = Url.Action("GoogleSignupCallback", "Authentication");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl! };
            properties.Items["role"] = role;
            return Challenge(properties, "Google");
        }

        [HttpGet("google-signup-callback")]
        public async Task<IActionResult> GoogleSignupCallback()
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync("Google");
                var desiredRole = result.Properties.Items["role"] ?? "Tenant";
                if (!result.Succeeded)
                    return BadRequest("Google authentication failed.");

                var email = result.Principal!.FindFirstValue(ClaimTypes.Email);
                if (email == null)
                    return BadRequest("No email claim from Google.");

                var name = email.Split("@")[0];

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {

                    var dto = new RegisterDTO { Email = email, Username = name ?? email, Password = Guid.NewGuid().ToString(), Role = desiredRole };
                    user = _mapper.Map<Tenant>(dto);
                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                        return BadRequest(createResult.Errors.Select(e => e.Description));
                    await _userManager.AddToRoleAsync(user, desiredRole);
                }
                else
                {
                    return BadRequest("user already exists");
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmUrl = Url.Action(
                    "ConfirmEmail",
                 "Authentication",
                    new { userId = user.Id, token },
                    Request.Scheme);

                await _authService.SendEmailConfirmation(user, confirmUrl);

                return Ok(new { Message = "registerd" });
            }
            catch(Exception e)
            {
                return BadRequest($"an unexpected error occured: {e}");
            }
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleLoginCallback", "Authentication");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl! };
            return Challenge(properties, "Google");
        }

        [HttpGet("google-login-callback")]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync("Google");

                if (!result.Succeeded)
                    return BadRequest("Google authentication failed.");


                var email = result.Principal!.FindFirstValue(ClaimTypes.Email);

                if (email == null)
                    return BadRequest("No email claim from Google.");


                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return BadRequest("user is not found");
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                    return BadRequest("You must confirm your email before logging in.");


                var (accessToken, refreshToken, refreshExpiry) = await _authService.GenerateTokens(user);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = refreshExpiry;
                await _userManager.UpdateAsync(user);

                return Ok(new { accessToken, refreshToken });

            }
            catch(Exception e)
            {
                return BadRequest($"an unexpected error occured: {e}");
            }
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

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(tenant);
                    var confirmUrl = Url.Action(
                        "ConfirmEmail",
                     "Authentication",
                        new { userId = tenant.Id, token },
                        Request.Scheme);

                    await _authService.SendEmailConfirmation(tenant, confirmUrl);
                }
                else if (_register.Role == "Owner")
                {
                    Owner owner = _mapper.Map<Owner>(_register);
                    var result = await _userManager.CreateAsync(owner, _register.Password);
                    if (!result.Succeeded)
                        return BadRequest(result.Errors.Select(e => e.Description));
                    await _userManager.AddToRoleAsync(owner, _register.Role);

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(owner);
                    var confirmUrl = Url.Action(
                        "ConfirmEmail",
                     "Authentication",
                        new { userId = owner.Id, token },
                        Request.Scheme);

                    await _authService.SendEmailConfirmation(owner, confirmUrl);
                }
                else if (_register.Role == "Admin")
                {
                    Admin admin = _mapper.Map<Admin>(_register);
                    var result = await _userManager.CreateAsync(admin, _register.Password);
                    if (!result.Succeeded)
                        return BadRequest(result.Errors.Select(e => e.Description));
                    await _userManager.AddToRoleAsync(admin, _register.Role);

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(admin);
                    var confirmUrl = Url.Action(
                        "ConfirmEmail",
                     "Authentication",
                        new { userId = admin.Id, token },
                        Request.Scheme);

                    await _authService.SendEmailConfirmation(admin, confirmUrl);
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

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest("You must confirm your email before logging in.");

            var IsCorrectPassword = await _userManager.CheckPasswordAsync(user, _login.Password);
            if (!IsCorrectPassword) return Unauthorized("Wrong Email or Password");

            var (accessToken, refreshToken, refreshExpiry) = await _authService.GenerateTokens(user);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshExpiry;
            await _userManager.UpdateAsync(user);

            return Ok(new { accessToken, refreshToken });

        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDTO req)
        {
            var principal = _authService.GetPrincipalFromExpiredToken(req.AccessToken);
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return BadRequest("Invalid access token");

            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized("User not found");

            
            if (user.RefreshToken != req.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token");

            
            var (accessToken, refreshToken, refreshExpiry) = await _authService.GenerateTokens(user);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshExpiry;
            await _userManager.UpdateAsync(user);

            return Ok(new { accessToken, refreshToken });
        }


        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest("Invalid confirmation request.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return BadRequest("Email confirmation failed.");

            // Optionally redirect to a static Angular page
            //    return Redirect("http://localhost:4200/email-confirmed");

            return Ok("Email confirmed successfully.");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return Ok("If the email is registered, a reset link has been sent.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action("ResetPassword", "Auth", new
            {
                token = token,
                email = user.Email
            }, Request.Scheme);

            // Send email with resetLink
            await _authService.SendResetPassword(user, resetLink);

            return Ok("If the email is registered, a reset link has been sent.");
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("Invalid request.");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password has been reset successfully.");
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDTO model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password changed successfully.");
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


