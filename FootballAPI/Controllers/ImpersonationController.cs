using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using FootballAPI.Service;
using FootballAPI.Models;
using FootballAPI.DTOs;

namespace FootballAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ADMIN")]
    public class ImpersonationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ImpersonationController> _logger;

        public ImpersonationController(
            IUserService userService,
            IConfiguration configuration,
            ILogger<ImpersonationController> logger)
        {
            _userService = userService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("start/{userId}")]
        public async Task<IActionResult> StartImpersonation(int userId)
        {
            try
            {
                // Get the admin user information
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(adminId))
                {
                    return Unauthorized(new { message = "Admin identity not found." });
                }

                // Get the user to impersonate
                var userToImpersonate = await _userService.GetUserByIdAsync(userId);
                if (userToImpersonate == null)
                {
                    return NotFound(new { message = "User to impersonate not found." });
                }

                // Generate impersonation token
                var token = GenerateImpersonationToken(userToImpersonate, int.Parse(adminId));

                return Ok(new
                {
                    message = "Impersonation successful.",
                    token,
                    user = new
                    {
                        id = userToImpersonate.Id,
                        email = userToImpersonate.Email,
                        role = userToImpersonate.Role,
                        username = userToImpersonate.Username
                    },
                    isImpersonating = true,
                    originalAdminId = adminId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during impersonation");
                return StatusCode(500, new { message = "An error occurred during impersonation." });
            }
        }

        public class StopImpersonationRequest
        {
            public string? OriginalAdminId { get; set; }
        }

        [HttpPost("stop")]
        [AllowAnonymous] // Allow anyone to stop impersonation
        public async Task<IActionResult> StopImpersonation([FromBody] StopImpersonationRequest request)
        {
            try
            {
                // Try to get admin ID from request body first, then from claims
                var originalAdminId = request?.OriginalAdminId;
                
                if (string.IsNullOrEmpty(originalAdminId))
                {
                    // Fallback to claim if not in request body
                    originalAdminId = User.FindFirst("OriginalAdminId")?.Value;
                }
                
                if (string.IsNullOrEmpty(originalAdminId))
                {
                    return BadRequest(new { message = "No active impersonation session found." });
                }

                // Get the admin user
                var adminUser = await _userService.GetUserByIdAsync(int.Parse(originalAdminId));
                if (adminUser == null)
                {
                    return NotFound(new { message = "Original admin user not found." });
                }

                // Generate token for the admin
                var token = GenerateRegularToken(adminUser);

                return Ok(new
                {
                    message = "Impersonation ended.",
                    token,
                    user = new
                    {
                        id = adminUser.Id,
                        email = adminUser.Email,
                        role = adminUser.Role,
                        username = adminUser.Username
                    },
                    isImpersonating = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ending impersonation");
                return StatusCode(500, new { message = "An error occurred while ending impersonation." });
            }
        }

        private string GenerateImpersonationToken(UserDto userToImpersonate, int adminId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"] ?? "your_super_secret_key_at_least_32_characters_long");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userToImpersonate.Id.ToString()),
                new Claim(ClaimTypes.Email, userToImpersonate.Email),
                new Claim(ClaimTypes.Role, userToImpersonate.Role.ToString()),
                new Claim(ClaimTypes.Name, userToImpersonate.Username),
                // Add special claims for impersonation
                new Claim("IsImpersonating", "true"),
                new Claim("OriginalAdminId", adminId.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(4), // Shorter expiry for impersonation
                Issuer = _configuration["JWT:Issuer"] ?? "yourdomain.com",
                Audience = _configuration["JWT:Audience"] ?? "yourdomain.com",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRegularToken(UserDto user)
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
            return tokenHandler.WriteToken(token);
        }
    }
}
