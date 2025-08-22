using FootballAPI.DTOs;
using FootballAPI.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FootballAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MatchHistoryController : ControllerBase
    {
        private readonly IMatchHistoryService _matchHistoryService;

        public MatchHistoryController(IMatchHistoryService matchHistoryService)
        {
            _matchHistoryService = matchHistoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchHistoryDto>>> GetAllMatchHistories()
        {
            try
            {
                var matchHistories = await _matchHistoryService.GetAllMatchHistoriesAsync();
                return Ok(matchHistories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving match histories", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MatchHistoryDto>> GetMatchHistoryById(int id)
        {
            try
            {
                var matchHistory = await _matchHistoryService.GetMatchHistoryByIdAsync(id);
                if (matchHistory == null)
                    return NotFound(new { message = "Match history not found" });

                return Ok(matchHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the match history", error = ex.Message });
            }
        }

        [HttpGet("match/{matchId}")]
        public async Task<ActionResult<MatchHistoryDto>> GetMatchHistoryByMatchId(int matchId)
        {
            try
            {
                var matchHistory = await _matchHistoryService.GetMatchHistoryByMatchIdAsync(matchId);
                if (matchHistory == null)
                    return NotFound(new { message = "Match history not found for this match" });

                return Ok(matchHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the match history", error = ex.Message });
            }
        }

        [HttpGet("player")]
        public async Task<ActionResult<IEnumerable<MatchHistorySummaryDto>>> GetPlayerMatchHistories()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new { message = "Invalid user token" });

                var matchHistories = await _matchHistoryService.GetPlayerMatchHistoriesAsync(userId);
                return Ok(matchHistories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving player match histories", error = ex.Message });
            }
        }

        [HttpGet("player/{playerId}")]
        public async Task<ActionResult<IEnumerable<MatchHistorySummaryDto>>> GetPlayerMatchHistories(int playerId)
        {
            try
            {
                var matchHistories = await _matchHistoryService.GetPlayerMatchHistoriesAsync(playerId);
                return Ok(matchHistories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving player match histories", error = ex.Message });
            }
        }

        [HttpGet("organiser")]
        public async Task<ActionResult<IEnumerable<MatchHistorySummaryDto>>> GetOrganiserMatchHistories()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new { message = "Invalid user token" });

                var matchHistories = await _matchHistoryService.GetOrganiserMatchHistoriesAsync(userId);
                return Ok(matchHistories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving organiser match histories", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<MatchHistoryDto>> CreateMatchHistory([FromBody] CreateMatchHistoryDto createMatchHistoryDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new { message = "Invalid user token" });

                var matchHistory = await _matchHistoryService.CreateMatchHistoryAsync(createMatchHistoryDto, userId);
                return CreatedAtAction(nameof(GetMatchHistoryById), new { id = matchHistory.Id }, matchHistory);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the match history", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MatchHistoryDto>> UpdateMatchHistory(int id, [FromBody] UpdateMatchHistoryDto updateMatchHistoryDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new { message = "Invalid user token" });

                var matchHistory = await _matchHistoryService.UpdateMatchHistoryAsync(id, updateMatchHistoryDto, userId);
                return Ok(matchHistory);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the match history", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMatchHistory(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new { message = "Invalid user token" });

                var success = await _matchHistoryService.DeleteMatchHistoryAsync(id, userId);
                if (!success)
                    return NotFound(new { message = "Match history not found" });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the match history", error = ex.Message });
            }
        }

        [HttpGet("player-stats")]
        public async Task<ActionResult<PlayerStatsAggregateDto>> GetPlayerStatsAggregate()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new { message = "Invalid user token" });

                var playerStats = await _matchHistoryService.GetPlayerStatsAggregateAsync(userId);
                return Ok(playerStats);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving player statistics", error = ex.Message });
            }
        }

        [HttpGet("player-stats/{playerId}")]
        public async Task<ActionResult<PlayerStatsAggregateDto>> GetPlayerStatsAggregate(int playerId)
        {
            try
            {
                var playerStats = await _matchHistoryService.GetPlayerStatsAggregateAsync(playerId);
                return Ok(playerStats);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving player statistics", error = ex.Message });
            }
        }
    }
}