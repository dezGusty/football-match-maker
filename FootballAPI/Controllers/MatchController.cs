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
    public class MatchController : ControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetAllMatches()
        {
            try
            {
                var matches = await _matchService.GetAllMatchesAsync();
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving matches", error = ex.Message });
            }
        }

        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PublicMatchDto>>> GetPublicMatches()
        {
            try
            {
                var matches = await _matchService.GetPublicMatchesAsync();
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving public matches", error = ex.Message });
            }
        }

        [HttpGet("organiser")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatchesByOrganiser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new { message = "Invalid user token" });

                var matches = await _matchService.GetMatchesByOrganiserAsync(userId);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving organiser matches", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MatchDto>> GetMatchById(int id)
        {
            try
            {
                var match = await _matchService.GetMatchByIdAsync(id);
                if (match == null)
                    return NotFound(new { message = "Match not found" });

                return Ok(match);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the match", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<MatchDto>> CreateMatch([FromBody] CreateMatchDto createMatchDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new { message = "Invalid user token" });

                var match = await _matchService.CreateMatchAsync(createMatchDto, userId);
                return CreatedAtAction(nameof(GetMatchById), new { id = match.Id }, match);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the match", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MatchDto>> UpdateMatch(int id, [FromBody] UpdateMatchDto updateMatchDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new { message = "Invalid user token" });

                var match = await _matchService.UpdateMatchAsync(id, updateMatchDto, userId);
                return Ok(match);
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
                return StatusCode(500, new { message = "An error occurred while updating the match", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMatch(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new { message = "Invalid user token" });

                var success = await _matchService.DeleteMatchAsync(id, userId);
                if (!success)
                    return NotFound(new { message = "Match not found" });

                return NoContent();
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
                return StatusCode(500, new { message = "An error occurred while deleting the match", error = ex.Message });
            }
        }

        [HttpPost("join")]
        public async Task<ActionResult<MatchPlayerDto>> JoinMatch([FromBody] JoinMatchDto joinMatchDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new { message = "Invalid user token" });

                var matchPlayer = await _matchService.JoinMatchAsync(joinMatchDto, userId);
                return Ok(matchPlayer);
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
                return StatusCode(500, new { message = "An error occurred while joining the match", error = ex.Message });
            }
        }

        [HttpPost("leave/{matchId}")]
        public async Task<ActionResult> LeaveMatch(int matchId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new { message = "Invalid user token" });

                var success = await _matchService.LeaveMatchAsync(matchId, userId);
                if (!success)
                    return NotFound(new { message = "Match not found or player not in match" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while leaving the match", error = ex.Message });
            }
        }

        [HttpPut("update-team")]
        public async Task<ActionResult> UpdatePlayerTeam([FromBody] UpdatePlayerTeamDto updatePlayerTeamDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new { message = "Invalid user token" });

                var success = await _matchService.UpdatePlayerTeamAsync(updatePlayerTeamDto, userId);
                if (!success)
                    return NotFound(new { message = "Match or player not found" });

                return NoContent();
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
                return StatusCode(500, new { message = "An error occurred while updating player team", error = ex.Message });
            }
        }

        [HttpGet("{matchId}/teams")]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetMatchTeams(int matchId)
        {
            try
            {
                var teams = await _matchService.GetMatchTeamsAsync(matchId);
                return Ok(teams);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving match teams", error = ex.Message });
            }
        }
    }
}