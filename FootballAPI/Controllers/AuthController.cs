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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, IUserService userService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _userService = userService;
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
            var role= User.FindFirst(ClaimTypes.Role)?.Value;
            var username= User.FindFirst(ClaimTypes.Name)?.Value;


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


        }



    }
