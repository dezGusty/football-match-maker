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
            return Ok(players ?? []);
        }

        [HttpPost("player-organiser")]
        public async Task<ActionResult> AddPlayerOrganiserRelation([FromBody] PlayerOrganiserRelationRequest request)
        {
            try
            {
                await _userService.AddPlayerOrganiserRelationAsync(request.UserId, request.OrganiserId);
                return Ok(new { message = "Player-organiser relation added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding player-organiser relation");
                return StatusCode(500, new { message = $"Internal error: {ex.Message}" });
            }
        }

        [HttpGet("players")]
        public async Task<ActionResult> GetAllPlayers()
        {
            try
            {
                var players = await _userService.GetPlayersAsync();
                return Ok(players);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all players");
                return StatusCode(500, new { message = $"Internal error: {ex.Message}" });
            }
        }

        [HttpPatch("{userId}/update-rating")]
        public async Task<ActionResult> UpdateUserRating(int userId, [FromBody] RatingUpdateRequest request)
        {
            try
            {
                var success = await _userService.UpdatePlayerRatingAsync(userId, request.RatingChange);
                if (success)
                    return Ok(new { message = "User rating updated successfully" });
                else
                    return BadRequest(new { message = "Failed to update user rating" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user rating");
                return StatusCode(500, new { message = $"Internal error: {ex.Message}" });
            }
        }

        [HttpPatch("update-multiple-ratings")]
        public async Task<ActionResult> UpdateMultipleUserRatings([FromBody] MultipleRatingUpdateRequest request)
        {
            try
            {
                var playerRatingUpdates = request.PlayerRatingUpdates.Select(x => new PlayerRatingUpdateDto
                {
                    UserId = x.UserId,
                    RatingChange = x.RatingChange
                }).ToList();

                var success = await _userService.UpdateMultiplePlayerRatingsAsync(playerRatingUpdates);
                if (success)
                    return Ok(new { message = "Multiple user ratings updated successfully" });
                else
                    return BadRequest(new { message = "Failed to update multiple user ratings" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating multiple user ratings");
                return StatusCode(500, new { message = $"Internal error: {ex.Message}" });
            }
        }


        [HttpPost("{id}/delegate-organizer-role")]
        public async Task<ActionResult<OrganizerDelegateDto>> DelegateOrganizerRole(int id, [FromBody] DelegateOrganizerRoleDto dto)
        {
            try
            {
                var delegation = await _userService.DelegateOrganizerRoleAsync(id, dto);
                return Ok(delegation);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error delegating organizer role");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation delegating organizer role");
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error delegating organizer role");
                return StatusCode(500, new { message = $"Internal error: {ex.Message}" });
            }
        }

        [HttpPost("{id}/reclaim-organizer-role")]
        public async Task<ActionResult> ReclaimOrganizerRole(int id, [FromBody] ReclaimOrganizerRoleDto dto)
        {
            try
            {
                var success = await _userService.ReclaimOrganizerRoleAsync(id, dto);
                if (success)
                    return Ok(new { message = "Organizer role reclaimed successfully" });
                else
                    return BadRequest(new { message = "Failed to reclaim organizer role" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reclaiming organizer role");
                return StatusCode(500, new { message = $"Internal error: {ex.Message}" });
            }
        }

        [HttpGet("{id}/delegation-status")]
        public async Task<ActionResult<DelegationStatusDto>> GetDelegationStatus(int id)
        {
            try
            {
                var status = await _userService.GetDelegationStatusAsync(id);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting delegation status");
                return StatusCode(500, new { message = $"Internal error: {ex.Message}" });
            }
        }

        [HttpGet("{id}/friends")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetFriends(int id)
        {
            try
            {
                var friends = await _userService.GetFriendsAsync(id);
                return Ok(friends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user friends");
                return StatusCode(500, new { message = $"Internal error: {ex.Message}" });
            }
        }
        [HttpPut("{id}/profile-image")]
        public async Task<IActionResult> UpdateProfileImage(int id, [FromForm] IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("No image file uploaded.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var imageUrl = $"{baseUrl}/images/{fileName}";

            var updatedUser = await _userService.UpdateUserProfileImageAsync(id, imageUrl);

            if (updatedUser == null)
                return NotFound($"User with ID {id} was not found.");

            return Ok(new { message = "Profile image updated successfully", imageUrl });
        }
        
        [HttpDelete("{id}/profile-image")]
        public async Task<IActionResult> DeleteProfileImage(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} was not found.");

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                var fileName = Path.GetFileName(user.ProfileImageUrl);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                
                user.ProfileImageUrl = null;
                await _userService.UpdateUserProfileImageAsync(id, null);
            }

            return Ok(new { message = "Profile image deleted successfully" });
        }




    }

    public class PlayerOrganiserRelationRequest
    {
        public int UserId { get; set; }
        public int OrganiserId { get; set; }
    }

    public class RatingUpdateRequest
    {
        public float RatingChange { get; set; }
    }

    public class MultipleRatingUpdateRequest
    {
        public List<UserRatingUpdate> PlayerRatingUpdates { get; set; } = new();
    }

    public class UserRatingUpdate
    {
        public int UserId { get; set; }
        public float RatingChange { get; set; }
    }
}