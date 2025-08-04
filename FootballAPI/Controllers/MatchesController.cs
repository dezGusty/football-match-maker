using Microsoft.AspNetCore.Mvc;
using FootballAPI.DTOs;
using FootballAPI.Service;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetAllMatches()
        {
            var matches = await _matchService.GetAllMatchesAsync();
            return Ok(matches);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MatchDto>> GetMatch(int id)
        {
            var match = await _matchService.GetMatchByIdAsync(id);
            if (match == null)
                return NotFound($"Match with ID {id} not found.");

            return Ok(match);
        }

        [HttpGet("daterange")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatchesByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var matches = await _matchService.GetMatchesByDateRangeAsync(startDate, endDate);
            return Ok(matches);
        }

        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatchesByTeam(int teamId)
        {
            var matches = await _matchService.GetMatchesByTeamIdAsync(teamId);
            return Ok(matches);
        }

        [HttpPost]
        public async Task<ActionResult<MatchDto>> CreateMatch(CreateMatchDto createMatchDto)
        {
            try
            {
                var match = await _matchService.CreateMatchAsync(createMatchDto);
                return CreatedAtAction(nameof(GetMatch), new { id = match.Id }, match);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating match: {ex.Message}");
            }
        }

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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMatch(int id)
        {
            var result = await _matchService.DeleteMatchAsync(id);
            if (!result)
                return NotFound($"Match with ID {id} not found.");

            return NoContent();
        }
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
        [HttpGet("past")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetPastMatches()
        {
            try
            {
                var pastMatches = await _matchService.GetPastMatchesAsync();
                return Ok(pastMatches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}