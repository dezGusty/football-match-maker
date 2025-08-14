using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Service;
using FootballAPI.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FootballAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IResetPasswordService _passwordResetService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            IUserService userService,
            IResetPasswordService passwordResetService,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _userService = userService;
            _passwordResetService = passwordResetService;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto.Email, dto.Password);
                if (token == null)
                    return Unauthorized(new { message = "Incorrect email or password." });

                _logger.LogInformation("Looking up user by email {Email}", dto.Email);
                var user = await _userService.GetUserByEmailAsync(dto.Email);
                _logger.LogInformation("User lookup result: {HasUser}", user != null);

                if (user == null)
                {

                    return StatusCode(500, new { message = "User not found after login." });
                }

                return Ok(new
                {
                    message = "Login successful.",
                    token,
                    user = new
                    {
                        id = user.Id,
                        email = user.Email,
                        role = user.Role,
                        username = user.Username
                    }
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Config/validation error during login for {Email}", dto.Email);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", dto.Email);
                return StatusCode(500, new { message = "An error occurred during login." });
            }
        }



        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync(HttpContext);
            return Ok(new { message = "Logout successful." });
        }


        [Authorize]
        [HttpGet("verify")]
        public IActionResult VerifyToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;


            return Ok(new
            {
                message = "Token is valid.",
                user = new
                {
                    id = userId,
                    email = email,
                    role = role,
                    username = username
                }
            });
        }

        // [Authorize] //Roles = "Admin" "Organiser")
        [HttpPost("create-player-account")]
        public async Task<IActionResult> CreatePlayerAccount([FromBody] CreatePlayerUserDto dto)
        {
            try
            {
                var existingUser = await _userService.GetUserByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "User with this email already exists." });
                }

                var user = new CreateUserDto
                {
                    Email = dto.Email,
                    Username = dto.Username,
                    Role = UserRole.PLAYER,
                    Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString())

                };

                var createdUser = await _userService.CreateUserAsync(user);

                var resetToken = await _passwordResetService.GeneratePasswordResetTokenAsync(createdUser.Id);

                var frontendUrl = _configuration["Frontend:BaseUrl"];

                var setPasswordUrl = $"{frontendUrl}/reset-password?token={resetToken}";

                // Trimitem email-ul
                await _emailService.SendSetPasswordEmailAsync(dto.Email, dto.Username, setPasswordUrl);

                _logger.LogInformation("Player account created and password reset email sent to {Email}", dto.Email);

                return Ok(new
                {
                    message = "Player account created successfully. Password setup email sent.",
                    userId = createdUser.Id
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating player account for {Email}", dto.Email);
                return StatusCode(500, new { message = "An error occurred while creating the account." });
            }
        }


        [HttpPost("set-password")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordDto dto)
        {
            try
            {
                var user = await _passwordResetService.GetUserByResetTokenAsync(dto.Token);
                if (user == null)
                {
                    return BadRequest(new { message = "Invalid or expired token." });
                }

                if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
                {
                    return BadRequest(new { message = "Password must be at least 6 characters long." });
                }

                var success = await _passwordResetService.ValidateAndResetPasswordAsync(dto.Token, dto.Password);
                if (!success)
                {
                    return BadRequest(new { message = "Failed to set password. Token may be invalid or expired." });
                }

                _logger.LogInformation("Password set successfully for user {UserId}", user.Id);

                return Ok(new { message = "Password set successfully. You can now login." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting password");
                return StatusCode(500, new { message = "An error occurred while setting the password." });
            }
        }



        [HttpGet("validate-reset-token/{token}")]
        public async Task<IActionResult> ValidateResetToken(string token)
        {
            try
            {
                var user = await _passwordResetService.GetUserByResetTokenAsync(token);
                if (user == null)
                {
                    return BadRequest(new { message = "Invalid or expired token.", isValid = false });
                }

                return Ok(new 
                { 
                    message = "Token is valid.",
                    isValid = true,
                    userEmail = user.Email,
                    username = user.Username
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating reset token");
                return StatusCode(500, new { message = "An error occurred while validating the token." });
            }
        }
    }
}
