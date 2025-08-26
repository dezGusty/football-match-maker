using Microsoft.AspNetCore.Mvc;
using FootballAPI.DTOs;
using FootballAPI.Service;
using FootballAPI.Repository;
using FootballAPI.Models;
using Microsoft.AspNetCore.Authorization;
using FootballAPI.Utils;

namespace FootballAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly IMatchService _matchService;

        public PlayersController(IPlayerService playerService, IMatchService matchService)
        {
            _playerService = playerService;
            _matchService = matchService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAllPlayers()
        {
            var players = await _playerService.GetAllPlayersAsync();
            return Ok(players);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDto>> GetPlayer(int id)
        {
            var player = await _playerService.GetPlayerByIdAsync(id);
            if (player == null)
                return NotFound($"Player with ID {id} not found.");

            return Ok(player);
        }


        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAvailablePlayers()
        {
            var players = await _playerService.GetAvailablePlayersAsync();
            return Ok(players);
        }

        [HttpGet("available/organiser/{organiserId}")]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAvailablePlayersByOrganiser(int organiserId)
        {
            var players = await _playerService.GetAvailablePlayersByOrganiserAsync(organiserId);
            return Ok(players);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> SearchPlayers([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term cannot be empty.");

            var players = await _playerService.SearchPlayersByNameAsync(searchTerm);
            return Ok(players);
        }



        [HttpPut("{id}")]
        public async Task<ActionResult<PlayerDto>> UpdatePlayer(int id, UpdatePlayerDto updatePlayerDto)
        {
            try
            {
                var player = await _playerService.UpdatePlayerAsync(id, updatePlayerDto);
                if (player == null)
                    return NotFound($"Player with ID {id} not found.");

                return Ok(player);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating player: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePlayer(int id)
        {
            var result = await _playerService.DeletePlayerAsync(id);
            if (!result)
                return NotFound($"Player with ID {id} not found.");

            return NoContent();
        }

        [HttpPatch("{id}/restore")]
        public async Task<ActionResult> RestorePlayer(int id)
        {
            var result = await _playerService.RestorePlayerAsync(id);
            if (!result)
                return NotFound($"Player with ID {id} not found or not deleted.");

            return NoContent();
        }

        [HttpPatch("{id}/update-rating")]
        public async Task<IActionResult> UpdatePlayerRating(int id, [FromBody] UpdatePlayerRatingDto updateRatingDto)
        {
            try
            {
                var result = await _playerService.UpdatePlayerRatingAsync(id, updateRatingDto.RatingChange);
                if (!result)
                {
                    return NotFound($"Player with ID {id} not found");
                }

                return Ok(new { message = "Player rating updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("update-multiple-ratings")]
        public async Task<IActionResult> UpdateMultiplePlayerRatings([FromBody] UpdateMultipleRatingsDto updateDto)
        {
            try
            {
                var result = await _playerService.UpdateMultiplePlayerRatingsAsync(updateDto.PlayerRatingUpdates);
                if (!result)
                {
                    return BadRequest("Failed to update player ratings");
                }

                return Ok(new { message = "Player ratings updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("player-organiser")]
        [Authorize]
        public async Task<IActionResult> AddPlayerOrganiserRelation([FromBody] PlayerOrganiserDto dto)
        {
            if (dto == null)
                return StatusCode(500, $"Invalid Data.");

            try
            {
                var organiserId = UserUtils.GetCurrentUserId(User, Request.Headers);
                await _playerService.AddPlayerOrganiserRelationAsync(dto.PlayerId, organiserId);
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch
            {
                return BadRequest("A player cannot create another player.");
            }
        }

        [HttpPost("{id}/profile-image")]
        public async Task<ActionResult> UpdatePlayerProfileImage(int id, IFormFile imageFile)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                    return BadRequest("No image file provided.");

                var imageUrl = await _playerService.UpdatePlayerProfileImageAsync(id, imageFile);
                return Ok(new { message = "Profile image updated successfully.", imageUrl });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating profile image: {ex.Message}");
            }
        }

        [HttpDelete("{id}/profile-image")]
        public async Task<ActionResult> DeletePlayerProfileImage(int id)
        {
            try
            {
                var success = await _playerService.DeletePlayerProfileImageAsync(id);
                if (!success)
                    return NotFound("Player not found or not enabled.");

                return Ok(new { message = "Profile image deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting profile image: {ex.Message}");
            }
        }

        [HttpGet("{id}/matches")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetPlayerMatches(int id)
        {
            try
            {
                var player = await _playerService.GetPlayerByIdAsync(id);
                if (player == null)
                    return NotFound($"Player with ID {id} not found.");


                var matches = await _matchService.GetPlayerMatchesAsync(id);

                return Ok(matches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting player matches: {ex.Message}");
            }
        }
    }
}