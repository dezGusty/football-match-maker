using Microsoft.AspNetCore.Mvc;
using FootballAPI.DTOs;
using FootballAPI.Service;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetTeam(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
                return NotFound($"Team with ID {id} not found.");

            return Ok(team);
        }

    }
} 