using API.DTOs.AuthenticationDTO;
using API.Models;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

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
                {
                    var msg = "Google signup failed";
                    var encodedMsg = WebUtility.UrlEncode(msg);
                    return Redirect($"{_config["ClientApp:BaseUrl"]}/google-signup-fail?msg={encodedMsg}");
                }

                var email = result.Principal!.FindFirstValue(ClaimTypes.Email);
                if (email == null)
                {
                    var msg = "No email by that name was found from Google";
                    var encodedMsg = WebUtility.UrlEncode(msg);
                    return Redirect($"{_config["ClientApp:BaseUrl"]}/google-signup-fail?msg={encodedMsg}");
                }

                var name = email.Split("@")[0];

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {

                    var dto = new RegisterDTO { Email = email, Username = name ?? email, Password = Guid.NewGuid().ToString(), Role = desiredRole };
                    user = _mapper.Map<Tenant>(dto);
                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        var msg = "an error occured while creating your account";
                        var encodedMsg = WebUtility.UrlEncode(msg);
                        return Redirect($"{_config["ClientApp:BaseUrl"]}/google-signup-fail?msg={encodedMsg}");
                    }
                    await _userManager.AddToRoleAsync(user, desiredRole);
                }
                else
                {
                    var msg = "this user already exists";
                    var encodedMsg = WebUtility.UrlEncode(msg);
                    return Redirect($"{_config["ClientApp:BaseUrl"]}/google-signup-fail?msg={encodedMsg}");
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmUrl = Url.Action(
                    "ConfirmEmail",
                    "Authentication",
                    new { userId = user.Id, token },
                    Request.Scheme);

                await _authService.SendEmailConfirmation(user, confirmUrl);

                return Redirect($"{_config["ClientApp:BaseUrl"]}/google-signup");
            }
            catch (Exception e)
            {
                var msg = $"an unexpected error occured {e.Message}";
                var encodedMsg = WebUtility.UrlEncode(msg);
                return Redirect($"{_config["ClientApp:BaseUrl"]}/google-signup-fail?msg={encodedMsg}");
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
                {
                    var msg = "Google login failed.";
                    var encodedMsg = WebUtility.UrlEncode(msg);
                    return Redirect($"{_config["ClientApp:BaseUrl"]}/google-login-fail?msg={encodedMsg}");
                }


                var email = result.Principal!.FindFirstValue(ClaimTypes.Email);

                if (email == null)
                {
                    var msg = "No email by that name was found from Google";
                    var encodedMsg = WebUtility.UrlEncode(msg);
                    return Redirect($"{_config["ClientApp:BaseUrl"]}/google-login-fail?msg={encodedMsg}");
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    var msg = "user is not found";
                    var encodedMsg = WebUtility.UrlEncode(msg);
                    return Redirect($"{_config["ClientApp:BaseUrl"]}/google-login-fail?msg={encodedMsg}");
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    var msg = "You must confirm your email before logging in.";
                    var encodedMsg = WebUtility.UrlEncode(msg);
                    return Redirect($"{_config["ClientApp:BaseUrl"]}/google-login-fail?msg={encodedMsg}");
                }

                var (accessToken, refreshToken, refreshExpiry) = await _authService.GenerateTokens(user);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = refreshExpiry;
                await _userManager.UpdateAsync(user);

                var encodedAccessToken = WebUtility.UrlEncode(accessToken);
                var encodedRefreshToken = WebUtility.UrlEncode(refreshToken);
                return Redirect($"{_config["ClientApp:BaseUrl"]}/google-login-success?accessToken={encodedAccessToken}&refreshToken={encodedRefreshToken}");

            }
            catch (Exception e)
            {
                var msg = "an unexpected error occured";
                var encodedMsg = WebUtility.UrlEncode(msg);
                return Redirect($"{_config["ClientApp:BaseUrl"]}/google-login-fail?msg={encodedMsg}");
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO _register)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not Valid Form Data!");
            var user = await _userManager.FindByEmailAsync(_register.Email);
            if (user != null) return BadRequest(new { msg = "This Email already Exists" });

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
                    return BadRequest(new { msg = "Invalid Role" });

                return Ok(new { msg = "registerd" });
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            ;

            return BadRequest(new { msg = "Cannot Register" });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO _login)
        {
            if (!ModelState.IsValid) return BadRequest(new { msg = "Not Valid Form Data!" });
            var user = await _userManager.FindByEmailAsync(_login.Email);
            if (user == null) return Unauthorized(new { msg = "This Email Doesn't Exists " });

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest(new { msg = "You must confirm your email before logging in." });

            var IsCorrectPassword = await _userManager.CheckPasswordAsync(user, _login.Password);
            if (!IsCorrectPassword) return Unauthorized(new { msg = "Wrong Email or Password" });

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
            if (userId == null) return BadRequest(new { msg = "Invalid access token" });


            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized(new { msg = "User not found" });


            if (user.RefreshToken != req.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return Unauthorized(new { msg = "Invalid or expired refresh token" });


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
                return BadRequest(new { msg = "Invalid confirmation request." });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { msg = "User not found." });

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return BadRequest(new { msg = "Email confirmation failed." });

            // Optionally redirect to a static Angular page
            return Redirect($"{_config["ClientApp:BaseUrl"]}/email-confirmed");

            //return Ok(new { msg = "Email confirmed successfully." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return Ok(new { msg = "If the email is registered, a reset link has been sent." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);


            var clientUrl = _config["ClientApp:BaseUrl"]!;


            var encodedToken = WebUtility.UrlEncode(token);
            var encodedEmail = WebUtility.UrlEncode(user.Email!);


            var resetLink = $"{clientUrl}/reset-password?token={encodedToken}&email={encodedEmail}";


            await _authService.SendResetPassword(user, resetLink);

            return Ok(new { msg = "If the email is registered, a reset link has been sent." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { msg = "Invalid request." });

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { msg = "Password has been reset successfully." });
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

            return Ok(new { msg = "Password changed successfully." });
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
/////////////////////////////////////////////////////////////////////////////////////////

//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using API.DTOs.AuthenticationDTO;
//using API.Models;
//using AutoMapper;

//namespace API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AuthenticationController : ControllerBase
//    {
//        public UserManager<ApplicationUser> _userManager { get; }
//        public IMapper _mapper { get; set; }

//        public AuthenticationController(UserManager<ApplicationUser> userManager, IMapper mapper)
//        {
//            _userManager = userManager;
//            _mapper = mapper;
//        }

//        [HttpPost("Register")]
//        public async Task<IActionResult> Register([FromBody] RegisterDTO _register)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var user = await _userManager.FindByEmailAsync(_register.Email);
//            if (user != null) return BadRequest("This Email Exists before");

//            try
//            {
//                if (_register.Role == "Tenant")
//                {
//                    Tenant tenant = _mapper.Map<Tenant>(_register);
//                    var result = await _userManager.CreateAsync(tenant, _register.Password);
//                    if (!result.Succeeded)
//                        return BadRequest(result.Errors.Select(e => e.Description));
//                    await _userManager.AddToRoleAsync(tenant, _register.Role);
//                }
//                else if (_register.Role == "Owner")
//                {
//                    Owner owner = _mapper.Map<Owner>(_register);
//                    var result = await _userManager.CreateAsync(owner, _register.Password);
//                    if (!result.Succeeded)
//                        return BadRequest(result.Errors.Select(e => e.Description));
//                    await _userManager.AddToRoleAsync(owner, _register.Role);
//                }
//                else if (_register.Role == "Admin")
//                {
//                    Admin admin = _mapper.Map<Admin>(_register);
//                    var result = await _userManager.CreateAsync(admin, _register.Password);
//                    if (!result.Succeeded)
//                        return BadRequest(result.Errors.Select(e => e.Description));
//                    await _userManager.AddToRoleAsync(admin, _register.Role);
//                }
//                else
//                    return BadRequest("Invalid Role");

//                return Ok(new { Message = "Registered successfully!" });
//            }
//            catch (Exception err)
//            {
//                Console.WriteLine(err);
//                return StatusCode(500, "An error occurred during registration.");
//            }
//        }


//        [HttpPost("Login")]
//        public async Task<IActionResult> Login([FromBody] LoginDTO _login)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var user = await _userManager.FindByEmailAsync(_login.Email);

//            if (user == null || !await _userManager.CheckPasswordAsync(user, _login.Password))
//            {
//                return Unauthorized("Invalid Email or Password");
//            }

//            var userData = new List<Claim>();
//            userData.Add(new Claim("userId", user.Id));
//            userData.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
//            userData.Add(new Claim("username", user.UserName));
//            userData.Add(new Claim("email", user.Email));

//            var roles = await _userManager.GetRolesAsync(user);
//            foreach (var role in roles)
//                userData.Add(new Claim(ClaimTypes.Role, role));

//            var key = "this is secrete key for admin role base";
//            var secreteKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
//            var signingCredentials = new SigningCredentials(secreteKey, SecurityAlgorithms.HmacSha256);

//            JwtSecurityToken tokenObject = new JwtSecurityToken(
//                claims: userData,
//                expires: DateTime.Now.AddDays(10000),
//                signingCredentials: signingCredentials
//            );

//            var token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
//            return Ok(new { token, roles = roles.ToList() });
//        }

//        [HttpGet("Tenant")]
//        [Authorize(Roles = "Tenant")]
//        public IActionResult Tenant()
//        {
//            return Ok("Authorized Tenant");
//        }

//        [HttpGet("Owner")]
//        [Authorize(Roles = "Owner")]
//        public IActionResult Owner()
//        {
//            return Ok("Authorized Owner");
//        }

//        [HttpGet("Admin")]
//        [Authorize(Roles = "Admin")]
//        public IActionResult Admin()
//        {
//            return Ok("Authorized Admin");
//        }
//    }
//}

