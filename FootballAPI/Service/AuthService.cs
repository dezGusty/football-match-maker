using FootballAPI.Repository;
using FootballAPI.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FootballAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;


        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null) return null;

            bool ok = false;
            try
            {
                ok = BCrypt.Net.BCrypt.Verify(password, user.Password);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                
                if (user.Password == password)
                {
                
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
                    await _userRepository.UpdateAsync(user);
                    ok = true;
                }
                else
                {
                    ok = false;
                }
            }

            if (!ok) return null;

            return GenerateJwtToken(user);
        }


        private string GenerateJwtToken(dynamic user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"] ?? "your_super_secret_key_at_least_32_characters_long");


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _configuration["JWT:Issuer"] ?? "yourdomain.com",
                Audience = _configuration["JWT:Audience"] ?? "yourdomain.com",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task LogoutAsync(HttpContext httpContext)
        {
            // Check if this is an impersonation session
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var isImpersonating = httpContext.User.FindFirst("IsImpersonating")?.Value;
                if (!string.IsNullOrEmpty(isImpersonating) && isImpersonating.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    // This is an impersonation session, get the impersonationLogId
                    var impersonationLogIdClaim = httpContext.User.FindFirst("ImpersonationLogId")?.Value;
                    var originalAdminIdClaim = httpContext.User.FindFirst("OriginalAdminId")?.Value;
                    
                    // If we have an ImpersonationLogService instance, use it to end the impersonation
                    var impersonationLogService = httpContext.RequestServices.GetService<IImpersonationLogService>();
                    if (impersonationLogService != null)
                    {
                        // Try to end by specific log ID first
                        if (!string.IsNullOrEmpty(impersonationLogIdClaim) && int.TryParse(impersonationLogIdClaim, out var logId))
                        {
                            await impersonationLogService.EndImpersonation(logId);
                        }
                        // Fallback to ending all active impersonations by this admin
                        else if (!string.IsNullOrEmpty(originalAdminIdClaim) && int.TryParse(originalAdminIdClaim, out var adminId))
                        {
                            await impersonationLogService.EndAllActiveImpersonationsByAdmin(adminId);
                        }
                    }
                }
            }
            
            await Task.CompletedTask;
        }
    }
}
