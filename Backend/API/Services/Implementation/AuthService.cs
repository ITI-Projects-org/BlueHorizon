using API.Models;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API.Services.Implementation
{
    public class AuthService : IAuthService
    {
        public UserManager<ApplicationUser> _userManager { get; }
        public IConfiguration _config { get; }
        public IEmailSender _emailSender { get; set; }

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config, IEmailSender emailSender)
        {
            _userManager = userManager;
            _config = config;
            _emailSender = emailSender;
        }
        public string GenerateRandomToken()
        {
            // 64 bytes → Base64 string
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<(string AccessToken, string RefreshToken, DateTime RefreshExpiry)> GenerateTokens(ApplicationUser user)
        {
            var userData = new List<Claim>();
            userData.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            userData.Add(new Claim("username", user.UserName));
            userData.Add(new Claim(ClaimTypes.Email, user.Email));


            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                userData.Add(new Claim(ClaimTypes.Role, role));


            #region SigningCredentials
            var key = _config["JwtKey"];
            var secreteKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            var signingCredentials = new SigningCredentials(secreteKey, SecurityAlgorithms.HmacSha256);
            #endregion

            JwtSecurityToken tokenObject = new JwtSecurityToken(
                claims: userData,
                // expires: DateTime.Now.AddMinutes(5),
                expires: DateTime.Now.AddMonths(12),
                signingCredentials: signingCredentials
                );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            var refreshToken = GenerateRandomToken();
            var refreshTokenExpiry = DateTime.Now.AddDays(7);

            return (accessToken, refreshToken, refreshTokenExpiry);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = _config["JwtKey"]!;
            var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = secretKey,
                ValidateLifetime = false  // we want to read even if expired
            };
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, tokenValidationParams, out var validatedToken);
            if (validatedToken is not JwtSecurityToken jwt ||
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }

        public async Task SendEmailConfirmation(ApplicationUser user, string confirmUrl)
        {
            await _emailSender.SendEmailAsync(
                user.Email,
                "Confirm Your email",
                $"Please confirm your account by <a href=\"{confirmUrl}\">clicking here</a>.");
        }

        public async Task SendResetPassword(ApplicationUser user, string resetLink)
        {
            await _emailSender.SendEmailAsync(
                user.Email,
                "Reset Your Password",
                $"Please reset your password by <a href=\"{resetLink}\">clicking here</a>.");
        }
    }
}
