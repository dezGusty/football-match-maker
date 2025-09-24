using Microsoft.AspNetCore.Mvc;
using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Models.Enums;
using FootballAPI.Service;
using Microsoft.AspNetCore.Authorization;
using FootballAPI.Utils;
using System.Security.Claims;
using FootballAPI.Repository;

namespace FootballAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchesController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetAllMatches()
        {
            var matches = await _matchService.GetAllMatchesAsync();
            return Ok(matches);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchDto>> GetMatch(int id)
        {
            var match = await _matchService.GetMatchByIdAsync(id);
            if (match == null)
                return NotFound($"Match with ID {id} not found.");

            return Ok(match);
        }

        [Authorize]
        [HttpGet("public")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetPublicMatches()
        {
            var matches = await _matchService.GetPublicMatchesAsync();
            return Ok(matches);
        }

        [Authorize(Roles = "ADMIN, ORGANISER")]
        [HttpPost]
        public async Task<ActionResult<MatchDto>> CreateMatch(CreateMatchDto createMatchDto)
        {
            try
            {
                var organiserId = UserUtils.GetCurrentUserId(User, Request.Headers);
                var match = await _matchService.CreateMatchAsync(createMatchDto, organiserId);
                return CreatedAtAction(nameof(GetMatch), new { id = match.Id }, match);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating match: {ex.Message}");
            }
        }

        [Authorize(Roles = "ADMIN, ORGANISER")]
        [HttpPut("{id}")]
        public async Task<ActionResult<MatchDto>> UpdateMatch(int id, UpdateMatchDto updateMatchDto)
        {

            try
            {
                var match = await _matchService.UpdateMatchAsync(id, updateMatchDto);
                if (match == null)
                    return NotFound($"Match with ID {id} not found.");

                return Ok(match);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating match: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("future")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetFutureMatches()
        {
            try
            {
                var futureMatches = await _matchService.GetFutureMatchesAsync();
                return Ok(futureMatches);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting future matches: {ex.Message}");
            }
        }

        [Authorize(Roles = "ADMIN, ORGANISER")]
        [HttpGet("past")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetPastMatches()
        {
            try
            {
                int id = UserUtils.GetCurrentUserId(User, Request.Headers);
                var pastMatches = await _matchService.GetPastMatchesAsync(id);
                return Ok(pastMatches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("past/my-matches")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMyPastMatches()
        {
            try
            {
                int userId = UserUtils.GetCurrentUserId(User, Request.Headers);
                var pastMatches = await _matchService.GetPastMatchesByParticipantAsync(userId);
                return Ok(pastMatches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "ADMIN, ORGANISER")]
        [HttpPost("{id}/players")]
        public async Task<ActionResult> AddPlayerToMatch(int id, AddPlayerToMatchDto addPlayerDto)
        {
            try
            {
                Console.WriteLine($"Adding player {addPlayerDto.UserId} to team {addPlayerDto.TeamId} in match {id}");
                var currentUserId = UserUtils.GetCurrentUserId(User, Request.Headers);
                var result = await _matchService.AddPlayerToTeamAsync(id, addPlayerDto.UserId, addPlayerDto.TeamId, currentUserId);
                if (!result)
                    return BadRequest("Could not add player to team. Team might be full or player already exists.");

                return Ok(new { message = "Player added successfully to team." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding player to match: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("{id}/join")]
        public async Task<ActionResult> JoinPublicMatch(int id)
        {
            try
            {
                var userId = UserUtils.GetCurrentUserId(User, Request.Headers);
                var result = await _matchService.JoinPublicMatchAsync(id, userId);
                if (!result)
                    return BadRequest("Could not join match. Match might be full, private, or player already in match.");

                return Ok(new { message = "Successfully joined the match." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error joining match: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("{matchId}/teams/{teamId}/join")]
        public async Task<ActionResult> JoinTeam(int matchId, int teamId)
        {
            try
            {
                var userId = UserUtils.GetCurrentUserId(User, Request.Headers);
                var result = await _matchService.JoinSpecificTeamAsync(matchId, userId, teamId);
                if (!result)
                    return BadRequest("Could not join team. Team might be full or player already in match.");

                return Ok(new { message = "Successfully joined the team." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error joining team: {ex.Message}");
            }
        }

        [Authorize(Roles = "ADMIN, ORGANISER")]
        [HttpPost("{id}/publish")]
        public async Task<ActionResult<MatchDto>> PublishMatch(int id)
        {
            try
            {
                var result = await _matchService.PublishMatchAsync(id);
                if (result == null)
                    return BadRequest("Could not publish match. Match needs at least 10 players.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error publishing match: {ex.Message}");
            }
        }

        [Authorize(Roles = "ADMIN, ORGANISER")]
        [HttpPost("{id}/unpublish")]
        public async Task<ActionResult<MatchDto>> UnpublishMatch(int id)
        {
            try
            {
                var result = await _matchService.MakeMatchPrivateAsync(id);
                if (result == null)
                    return BadRequest("Could not make match private. Match not found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error making match private: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("{id}/details")]
        public async Task<ActionResult<MatchDetailsDto>> GetMatchDetails(int id)
        {
            try
            {
                var result = await _matchService.GetMatchDetailsAsync(id);
                if (result == null)
                    return NotFound($"Match with ID {id} not found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting match details: {ex.Message}");
            }
        }

        [Authorize]
        [HttpDelete("{id}/leave")]
        public async Task<ActionResult> LeaveMatch(int id)
        {
            try
            {
                var userId = UserUtils.GetCurrentUserId(User, Request.Headers);
                var result = await _matchService.LeaveMatchAsync(id, userId);

                if (!result)
                    return BadRequest("Could not leave match. You might not be part of this match.");

                return Ok(new { message = "Successfully left the match." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error leaving match: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("my-matches")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMyMatches()
        {
            try
            {
                var userId = UserUtils.GetCurrentUserId(User, Request.Headers);
                var matches = await _matchService.GetPlayerMatchesAsync(userId);
                return Ok(matches);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting player matches: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetAvailableMatches()
        {
            try
            {
                var userId = UserUtils.GetCurrentUserId(User, Request.Headers);
                var matches = await _matchService.GetAvailableMatchesForPlayerAsync(userId);
                return Ok(matches);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting available matches: {ex.Message}");
            }
        }

        [HttpGet("my-public-matches")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMyPublicMatches()
        {
            try
            {
                var userId = UserUtils.GetCurrentUserId(User, Request.Headers);
                var matches = await _matchService.GetMyPublicMatchesAsync(userId);
                return Ok(matches);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting my public matches: {ex.Message}");
            }
        }

        [Authorize(Roles = "ADMIN, ORGANISER")]
        [HttpGet("organiser/matches")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatchesByOrganiserAsync()
        {
            try
            {
                var organiserId = UserUtils.GetCurrentUserId(User, Request.Headers);
                var matches = await _matchService.GetMatchesByOrganiserAsync(organiserId);
                return Ok(matches);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting matches by organiser: {ex.Message}");
            }
        }

        [Authorize(Roles = "ADMIN, ORGANISER")]
        [HttpPost("{id}/close")]
        public async Task<ActionResult<MatchDto>> CloseMatch(int id)
        {
            try
            {
                var result = await _matchService.CloseMatchAsync(id);
                if (result == null)
                    return BadRequest("Could not close match. Match needs at least 10 players or is not in Open status.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error closing match: {ex.Message}");
            }
        }

        [HttpPost("{id}/cancel")]
        [Authorize]
        public async Task<ActionResult<MatchDto>> CancelMatch(int id)
        {
            try
            {
                var result = await _matchService.CancelMatchAsync(id);
                if (result == null)
                    return BadRequest("Could not cancel match. Match not found or invalid status.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error cancelling match: {ex.Message}");
            }
        }

        [Authorize(Roles = "ADMIN, ORGANISER")]
        [HttpDelete("{matchId}/players/{userId}")]
        public async Task<ActionResult> RemovePlayerFromMatch(int matchId, int userId)
        {
            try
            {
                var result = await _matchService.RemovePlayerFromMatchAsync(matchId, userId);
                if (!result)
                    return BadRequest("Could not remove player from match. Player might not be in this match.");

                return Ok(new { message = "Player removed successfully from match." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error removing player from match: {ex.Message}");
            }
        }

        [Authorize(Roles = "ADMIN, ORGANISER")]
        [HttpPost("{id}/rating-preview")]
        public async Task<ActionResult<IEnumerable<RatingPreviewDto>>> CalculateRatingPreview(int id, CalculateRatingPreviewDto dto)
        {
            try
            {
                var ratingPreviews = await _matchService.CalculateRatingPreviewAsync(id, dto);
                return Ok(ratingPreviews);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error calculating rating preview: {ex.Message}");
            }
        }

        [Authorize(Roles = "ADMIN, ORGANISER")]
        [HttpPut("finalize/{id}")]
        public async Task<ActionResult<MatchDto>> FinalizeMatch(int id, FinalizeMatchDto finalizeMatchDto)
        {
            try
            {
                var match = await _matchService.FinalizeMatchAsync(id, finalizeMatchDto);
                if (match == null)
                    return NotFound($"Match with ID {id} not found.");

                return Ok(match);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error finalizing match: {ex.Message}");
            }
        }

    }
}