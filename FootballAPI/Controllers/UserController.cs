using FootballAPI.DTOs;
using FootballAPI.Service;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// Obține toți utilizatorii
        /// </summary>
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
                return StatusCode(500, $"Eroare internă: {ex.Message}");
            }
        }

        /// <summary>
        /// Obține un utilizator după ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound($"User cu ID {id} nu a fost găsit.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {Id}", id);
                return StatusCode(500, $"Eroare internă: {ex.Message}");
            }
        }

        /// <summary>
        /// Obține un utilizator după username
        /// </summary>
        [HttpGet("username/{username}")]
        public async Task<ActionResult<UserDto>> GetUserByUsername(string username)
        {
            try
            {
                var user = await _userService.GetUserByUsernameAsync(username);

                if (user == null)
                {
                    return NotFound($"User cu username '{username}' nu a fost găsit.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username: {Username}", username);
                return StatusCode(500, $"Eroare internă: {ex.Message}");
            }
        }

        /// <summary>
        /// Obține utilizatorii după rol
        /// </summary>
        [HttpGet("role/{role}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(string role)
        {
            try
            {
                var users = await _userService.GetUsersByRoleAsync(role);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by role: {Role}", role);
                return StatusCode(500, $"Eroare internă: {ex.Message}");
            }
        }

        /// <summary>
        /// Creează un nou utilizator
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdUser = await _userService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUserById),
                    new { id = createdUser.Id }, createdUser);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating user");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, $"Eroare internă: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualizează un utilizator
        /// </summary>
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
                    return NotFound($"User cu ID {id} nu a fost găsit.");
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
                return StatusCode(500, $"Eroare internă: {ex.Message}");
            }
        }

        /// <summary>
        /// Șterge un utilizator
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var deleted = await _userService.DeleteUserAsync(id);

                if (!deleted)
                {
                    return NotFound($"User cu ID {id} nu a fost găsit.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return StatusCode(500, $"Eroare internă: {ex.Message}");
            }
        }

        /// <summary>
        /// Autentificare utilizator
        /// </summary>
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
                    return Unauthorized("Username sau password incorect.");
                }

                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating user");
                return StatusCode(500, $"Eroare internă: {ex.Message}");
            }
        }

        /// <summary>
        /// Schimbă parola unui utilizator
        /// </summary>
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
                    return NotFound($"User cu ID {id} nu a fost găsit.");
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
                return StatusCode(500, $"Eroare internă: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifică dacă un username există
        /// </summary>
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
                return StatusCode(500, $"Eroare internă: {ex.Message}");
            }
        }
    }
}