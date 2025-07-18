using Microsoft.AspNetCore.Mvc;
using FootballAPI.DTOs;
using FootballAPI.Service;

namespace FootballAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetAllTeams()
        {
            var teams = await _teamService.GetAllTeamsAsync();
            return Ok(teams);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetTeam(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
                return NotFound($"Team with ID {id} not found.");

            return Ok(team);
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<TeamDto>> GetTeamByName(string name)
        {
            var team = await _teamService.GetTeamByNameAsync(name);
            if (team == null)
                return NotFound($"Team with name {name} not found.");

            return Ok(team);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TeamDto>>> SearchTeams([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term cannot be empty.");

            var teams = await _teamService.SearchTeamsByNameAsync(searchTerm);
            return Ok(teams);
        }

        [HttpPost]
        public async Task<ActionResult<TeamDto>> CreateTeam(CreateTeamDto createTeamDto)
        {
            try
            {
                var team = await _teamService.CreateTeamAsync(createTeamDto);
                return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating team: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TeamDto>> UpdateTeam(int id, UpdateTeamDto updateTeamDto)
        {
            try
            {
                var team = await _teamService.UpdateTeamAsync(id, updateTeamDto);
                if (team == null)
                    return NotFound($"Team with ID {id} not found.");

                return Ok(team);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating team: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTeam(int id)
        {
            var result = await _teamService.DeleteTeamAsync(id);
            if (!result)
                return NotFound($"Team with ID {id} not found.");

            return NoContent();
        }
    }
} 