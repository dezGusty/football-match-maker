using FootballAPI.Models;
using FootballAPI.Service;
using FootballAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FootballAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerMatchHistoryController : ControllerBase
    {
        private readonly IPlayerMatchHistoryService _service;
        private readonly ILogger<PlayerMatchHistoryController> _logger;

        public PlayerMatchHistoryController(IPlayerMatchHistoryService service, ILogger<PlayerMatchHistoryController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<PlayerMatchHistoryDto>> CreatePlayerMatchHistory([FromBody] CreatePlayerMatchHistoryDto createDto)
        {
            try
            {
                _logger.LogInformation("Attempting to create PlayerMatchHistory with data: PlayerId={PlayerId}, TeamId={TeamId}, MatchId={MatchId}, PerformanceRating={PerformanceRating}",
                    createDto.PlayerId, createDto.TeamId, createDto.MatchId, createDto.PerformanceRating);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid");
                    return BadRequest(ModelState);
                }

                var playerMatchHistory = new PlayerMatchHistory
                {
                    PlayerId = createDto.PlayerId,
                    TeamId = createDto.TeamId,
                    MatchId = createDto.MatchId,
                    PerformanceRating = createDto.PerformanceRating,
                    RecordDate = DateTime.Now
                };

                _logger.LogInformation("Calling service to create PlayerMatchHistory");
                var createdPlayerMatchHistory = await _service.CreatePlayerMatchHistoryAsync(playerMatchHistory);

                _logger.LogInformation("Successfully created PlayerMatchHistory with ID: {Id}", createdPlayerMatchHistory.Id);

                return CreatedAtAction(nameof(GetPlayerMatchHistoryById),
                    new { id = createdPlayerMatchHistory.Id }, createdPlayerMatchHistory);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "ArgumentException occurred while creating PlayerMatchHistory");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating PlayerMatchHistory. InnerException: {InnerException}", ex.InnerException?.Message);
                return StatusCode(500, $"Internal error: {ex.Message}. Inner: {ex.InnerException?.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerMatchHistoryDto>>> GetAllPlayerMatchHistory()
        {
            try
            {
                var playerMatchHistory = await _service.GetAllPlayerMatchHistoryAsync();
                return Ok(playerMatchHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all PlayerMatchHistory");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerMatchHistoryDto>> GetPlayerMatchHistoryById(int id)
        {
            try
            {
                var playerMatchHistory = await _service.GetPlayerMatchHistoryByIdAsync(id);

                if (playerMatchHistory == null)
                {
                    return NotFound($"PlayerMatchHistory with ID {id} was not found.");
                }

                return Ok(playerMatchHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting PlayerMatchHistory by ID: {Id}", id);
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpGet("player/{playerId}")]
        public async Task<ActionResult<IEnumerable<PlayerMatchHistoryDto>>> GetPlayerMatchHistoryByPlayerId(int playerId)
        {
            try
            {
                var playerMatchHistory = await _service.GetPlayerMatchHistoryByPlayerIdAsync(playerId);
                return Ok(playerMatchHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting PlayerMatchHistory by PlayerId: {PlayerId}", playerId);
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<IEnumerable<PlayerMatchHistoryDto>>> GetPlayerMatchHistoryByTeamId(int teamId)
        {
            try
            {
                var playerMatchHistory = await _service.GetPlayerMatchHistoryByTeamIdAsync(teamId);
                return Ok(playerMatchHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting PlayerMatchHistory by TeamId: {TeamId}", teamId);
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpGet("match/{matchId}")]
        public async Task<ActionResult<IEnumerable<PlayerMatchHistoryDto>>> GetPlayerMatchHistoryByMatchId(int matchId)
        {
            try
            {
                var playerMatchHistory = await _service.GetPlayerMatchHistoryByMatchIdAsync(matchId);
                return Ok(playerMatchHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting PlayerMatchHistory by MatchId: {MatchId}", matchId);
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PlayerMatchHistoryDto>> UpdatePlayerMatchHistory(int id, [FromBody] UpdatePlayerMatchHistoryDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var playerMatchHistory = new PlayerMatchHistory
                {
                    Id = id,
                    PlayerId = updateDto.PlayerId,
                    TeamId = updateDto.TeamId,
                    MatchId = updateDto.MatchId,
                    PerformanceRating = updateDto.PerformanceRating
                };

                var updatedPlayerMatchHistory = await _service.UpdatePlayerMatchHistoryAsync(id, playerMatchHistory);

                if (updatedPlayerMatchHistory == null)
                {
                    return NotFound($"PlayerMatchHistory with ID {id} was not found.");
                }

                return Ok(updatedPlayerMatchHistory);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "ArgumentException occurred while updating PlayerMatchHistory");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating PlayerMatchHistory");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePlayerMatchHistory(int id)
        {
            try
            {
                var deleted = await _service.DeletePlayerMatchHistoryAsync(id);

                if (!deleted)
                {
                    return NotFound($"PlayerMatchHistory with ID {id} was not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting PlayerMatchHistory");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpGet("player/{playerId}/average-rating")]
        public async Task<ActionResult<float>> GetAveragePerformanceRating(int playerId)
        {
            try
            {
                var averageRating = await _service.GetAveragePerformanceRatingAsync(playerId);
                return Ok(new { PlayerId = playerId, AverageRating = averageRating });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting average performance rating");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpGet("top-performances")]
        public async Task<ActionResult<IEnumerable<PlayerMatchHistoryDto>>> GetTopPerformances(int count = 10)
        {
            try
            {
                var topPerformances = await _service.GetTopPerformancesAsync(count);
                return Ok(topPerformances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top performances");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }
    }
}