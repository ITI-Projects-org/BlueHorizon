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
            var htmlTemplate = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Email Confirmation - BlueHorizon</title>
    <style>
        body {{
            margin: 0;
            padding: 0;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f8f9fa;
            line-height: 1.6;
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 12px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, #007bff 0%, #0056b3 100%);
            color: white;
            padding: 40px 30px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 28px;
            font-weight: 600;
        }}
        .header p {{
            margin: 10px 0 0 0;
            opacity: 0.9;
            font-size: 16px;
        }}
        .content {{
            padding: 40px 30px;
        }}
        .welcome-message {{
            font-size: 18px;
            color: #333;
            margin-bottom: 25px;
        }}
        .welcome-message strong {{
            color: #007bff;
        }}
        .message {{
            color: #666;
            margin-bottom: 30px;
            font-size: 16px;
        }}
        .cta-button {{
            display: inline-block;
            background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
            color: white;
            text-decoration: none;
            padding: 15px 35px;
            border-radius: 8px;
            font-weight: 600;
            font-size: 16px;
            margin: 20px 0;
            transition: transform 0.2s ease;
        }}
        .cta-button:hover {{
            transform: translateY(-2px);
            text-decoration: none;
            color: white;
        }}
        .security-note {{
            background-color: #f8f9fa;
            border-left: 4px solid #007bff;
            padding: 15px 20px;
            margin: 25px 0;
            border-radius: 4px;
        }}
        .security-note h4 {{
            margin: 0 0 10px 0;
            color: #333;
            font-size: 16px;
        }}
        .security-note p {{
            margin: 0;
            font-size: 14px;
            color: #666;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 25px 30px;
            text-align: center;
            border-top: 1px solid #e9ecef;
        }}
        .footer p {{
            margin: 5px 0;
            color: #6c757d;
            font-size: 14px;
        }}
        .footer .brand {{
            color: #007bff;
            font-weight: 600;
        }}
        .icon {{
            font-size: 48px;
            margin-bottom: 20px;
        }}
        @media (max-width: 600px) {{
            .container {{
                margin: 10px;
                border-radius: 8px;
            }}
            .header, .content, .footer {{
                padding: 25px 20px;
            }}
            .header h1 {{
                font-size: 24px;
            }}
            .cta-button {{
                display: block;
                text-align: center;
                margin: 20px 0;
            }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <div class=""icon"">🏠</div>
            <h1>Welcome to BlueHorizon!</h1>
            <p>Your premium real estate booking platform</p>
        </div>
        
        <div class=""content"">
            <div class=""welcome-message"">
                Hello <strong>{user.UserName ?? "Valued User"}</strong>,
            </div>
            
            <p class=""message"">
                Thank you for joining BlueHorizon! We're excited to have you as part of our community. 
                To complete your registration and start exploring amazing properties, please confirm your email address.
            </p>
            
            <div style=""text-align: center;"">
                <a href=""{confirmUrl}"" class=""cta-button"">
                    ✅ Confirm My Email Address
                </a>
            </div>
            
            <div class=""security-note"">
                <h4>🔒 Security Information</h4>
                <p>
                    This confirmation link will expire in 24 hours for your security. 
                    If you didn't create an account with BlueHorizon, please ignore this email.
                </p>
            </div>
            
            <p class=""message"">
                Once confirmed, you'll be able to:
            </p>
            <ul style=""color: #666; margin-left: 20px;"">
                <li>Browse premium properties</li>
                <li>Make instant bookings</li>
                <li>Manage your reservations</li>
                <li>Access exclusive deals</li>
            </ul>
        </div>
        
        <div class=""footer"">
            <p class=""brand"">BlueHorizon Team</p>
            <p>Premium Real Estate Booking Platform</p>
            <p style=""margin-top: 15px; font-size: 12px;"">
                If you're having trouble with the button above, copy and paste this link into your browser:
            </p>
            <p style=""word-break: break-all; color: #007bff; font-size: 12px;"">
                {confirmUrl}
            </p>
        </div>
    </div>
</body>
</html>";

            await _emailSender.SendEmailAsync(
                user.Email,
                "Welcome to BlueHorizon - Please Confirm Your Email",
                htmlTemplate);
        }

        public async Task SendResetPassword(ApplicationUser user, string resetLink)
        {
            var htmlTemplate = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Password Reset - BlueHorizon</title>
    <style>
        body {{
            margin: 0;
            padding: 0;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f8f9fa;
            line-height: 1.6;
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 12px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, #dc3545 0%, #c82333 100%);
            color: white;
            padding: 40px 30px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 28px;
            font-weight: 600;
        }}
        .header p {{
            margin: 10px 0 0 0;
            opacity: 0.9;
            font-size: 16px;
        }}
        .content {{
            padding: 40px 30px;
        }}
        .greeting {{
            font-size: 18px;
            color: #333;
            margin-bottom: 25px;
        }}
        .greeting strong {{
            color: #dc3545;
        }}
        .message {{
            color: #666;
            margin-bottom: 30px;
            font-size: 16px;
        }}
        .cta-button {{
            display: inline-block;
            background: linear-gradient(135deg, #dc3545 0%, #c82333 100%);
            color: white;
            text-decoration: none;
            padding: 15px 35px;
            border-radius: 8px;
            font-weight: 600;
            font-size: 16px;
            margin: 20px 0;
            transition: transform 0.2s ease;
        }}
        .cta-button:hover {{
            transform: translateY(-2px);
            text-decoration: none;
            color: white;
        }}
        .warning-box {{
            background-color: #fff3cd;
            border: 1px solid #ffeaa7;
            border-left: 4px solid #ffc107;
            padding: 15px 20px;
            margin: 25px 0;
            border-radius: 4px;
        }}
        .warning-box h4 {{
            margin: 0 0 10px 0;
            color: #856404;
            font-size: 16px;
        }}
        .warning-box p {{
            margin: 0;
            font-size: 14px;
            color: #856404;
        }}
        .security-tips {{
            background-color: #d1ecf1;
            border: 1px solid #bee5eb;
            border-left: 4px solid #17a2b8;
            padding: 15px 20px;
            margin: 25px 0;
            border-radius: 4px;
        }}
        .security-tips h4 {{
            margin: 0 0 10px 0;
            color: #0c5460;
            font-size: 16px;
        }}
        .security-tips ul {{
            margin: 10px 0 0 0;
            padding-left: 20px;
            color: #0c5460;
        }}
        .security-tips li {{
            margin: 5px 0;
            font-size: 14px;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 25px 30px;
            text-align: center;
            border-top: 1px solid #e9ecef;
        }}
        .footer p {{
            margin: 5px 0;
            color: #6c757d;
            font-size: 14px;
        }}
        .footer .brand {{
            color: #dc3545;
            font-weight: 600;
        }}
        .icon {{
            font-size: 48px;
            margin-bottom: 20px;
        }}
        .expiry-notice {{
            background-color: #f8d7da;
            border: 1px solid #f5c6cb;
            color: #721c24;
            padding: 12px 15px;
            border-radius: 4px;
            margin: 20px 0;
            text-align: center;
            font-size: 14px;
            font-weight: 600;
        }}
        @media (max-width: 600px) {{
            .container {{
                margin: 10px;
                border-radius: 8px;
            }}
            .header, .content, .footer {{
                padding: 25px 20px;
            }}
            .header h1 {{
                font-size: 24px;
            }}
            .cta-button {{
                display: block;
                text-align: center;
                margin: 20px 0;
            }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <div class=""icon"">🔐</div>
            <h1>Password Reset Request</h1>
            <p>BlueHorizon Security Center</p>
        </div>
        
        <div class=""content"">
            <div class=""greeting"">
                Hello <strong>{user.UserName ?? "Valued User"}</strong>,
            </div>
            
            <p class=""message"">
                We received a request to reset the password for your BlueHorizon account. 
                If you made this request, click the button below to create a new password.
            </p>
            
            <div style=""text-align: center;"">
                <a href=""{resetLink}"" class=""cta-button"">
                    🔑 Reset My Password
                </a>
            </div>
            
            <div class=""expiry-notice"">
                ⏰ This link will expire in 1 hour for your security
            </div>
            
            <div class=""warning-box"">
                <h4>⚠️ Important Security Notice</h4>
                <p>
                    If you didn't request a password reset, please ignore this email. Your account remains secure, 
                    and no changes have been made.
                </p>
            </div>
            
            <div class=""security-tips"">
                <h4>🛡️ Password Security Tips</h4>
                <ul>
                    <li>Use at least 8 characters with mixed case letters, numbers, and symbols</li>
                    <li>Avoid using personal information or common words</li>
                    <li>Don't reuse passwords from other accounts</li>
                    <li>Consider using a password manager</li>
                </ul>
            </div>
            
            <p class=""message"">
                If you continue to have problems, please contact our support team. We're here to help!
            </p>
        </div>
        
        <div class=""footer"">
            <p class=""brand"">BlueHorizon Security Team</p>
            <p>Keeping your account safe and secure</p>
            <p style=""margin-top: 15px; font-size: 12px;"">
                If you're having trouble with the button above, copy and paste this link into your browser:
            </p>
            <p style=""word-break: break-all; color: #dc3545; font-size: 12px;"">
                {resetLink}
            </p>
            <p style=""margin-top: 15px; font-size: 12px; color: #999;"">
                This email was sent from a no-reply address. Please do not reply to this email.
            </p>
        </div>
    </div>
</body>
</html>";

            await _emailSender.SendEmailAsync(
                user.Email,
                "BlueHorizon - Password Reset Request",
                htmlTemplate);
        }
    }
}
