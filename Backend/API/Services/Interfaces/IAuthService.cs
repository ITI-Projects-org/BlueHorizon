using API.Models;
using System.Security.Claims;

namespace API.Services.Interfaces
{
    public interface IAuthService
    {
        string GenerateRandomToken();
        Task<(string AccessToken, string RefreshToken, DateTime RefreshExpiry)> GenerateTokens(ApplicationUser user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task SendEmailConfirmation(ApplicationUser user, string confirmUrl);
        Task SendResetPassword(ApplicationUser user, string resetLink);
    }
}
