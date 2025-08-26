using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Models.Enums;
using FootballAPI.Service;
using FootballAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace FootballAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound($"User with ID {id} was not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {Id}", id);
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpGet("username/{username}")]
        public async Task<ActionResult<UserDto>> GetUserByUsername(string username)
        {
            try
            {
                var user = await _userService.GetUserByUsernameAsync(username);

                if (user == null)
                {
                    return NotFound($"User with username '{username}' was not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username: {Username}", username);
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);

                if (user == null)
                {
                    return NotFound($"User with email '{email}' was not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpGet("role/{role}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(UserRole role)
        {
            try
            {
                var users = await _userService.GetUsersByRoleAsync(role);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by role: {Role}", role);
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto dto)
        {
            var user = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPost("create-player-user")]
        public async Task<ActionResult<UserDto>> CreatePlayerUser([FromBody] CreatePlayerUserDto dto)
        {
            try
            {
                int? organizerId = null;

                try
                {
                    organizerId = UserUtils.GetCurrentUserId(User, Request.Headers);
                }
                catch (UnauthorizedAccessException)
                {
                    organizerId = null;
                }

                var user = await _userService.CreatePlayerUserAsync(dto, organizerId);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating player user");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedUser = await _userService.UpdateUserAsync(id, updateUserDto);

                if (updatedUser == null)
                {
                    return NotFound($"User with ID {id} was not found.");
                }

                return Ok(updatedUser);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error updating user");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var deleted = await _userService.DeleteUserAsync(id);

                if (!deleted)
                {
                    return NotFound($"User with ID {id} was not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<UserResponseDto>> Authenticate([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userResponse = await _userService.AuthenticateAsync(loginDto);

                if (userResponse == null)
                {
                    return Unauthorized("Incorrect username or password.");
                }

                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating user");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpPost("{id}/change-password")]
        public async Task<ActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _userService.ChangePasswordAsync(id, changePasswordDto);

                if (!success)
                {
                    return NotFound($"User with ID {id} was not found.");
                }

                return Ok(new { message = "Password changed successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error changing password");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpGet("check-username/{username}")]
        public async Task<ActionResult<bool>> CheckUsernameExists(string username)
        {
            try
            {
                var exists = await _userService.UsernameExistsAsync(username);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking username existence");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpPost("{id}/change-username")]
        public async Task<ActionResult> ChangeUsername(int id, [FromBody] ChangeUsernameDto changeUsernameDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _userService.ChangeUsernameAsync(id, changeUsernameDto);

                if (!success)
                {
                    return NotFound($"User with ID {id} was not found.");
                }

                return Ok(new { message = "Username changed successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error changing username");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing username");
                return StatusCode(500, new { message = $"Internal error: {ex.Message}" });
            }
        }

        [HttpGet("{id}/players")]
        public async Task<ActionResult> GetPlayersForOrganiser(int id)
        {
            var players = await _userService.GetPlayersByOrganiserAsync(id);
            if (players == null || !players.Any())
                return NotFound("No players found for this organiser.");

            return Ok(players);
        }
    }
}