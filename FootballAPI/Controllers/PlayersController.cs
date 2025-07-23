using Microsoft.AspNetCore.Mvc;
using FootballAPI.DTOs;
using FootballAPI.Service;
using FootballAPI.Repository;
namespace FootballAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayersController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAllPlayers()
        {
            var players = await _playerService.GetAllPlayersAsync();
            return Ok(players);
        }

        [HttpGet("with-images")]
        public async Task<ActionResult<IEnumerable<PlayerWithImageDto>>> GetAllPlayersWithImages()
        {
            var players = await _playerService.GetAllPlayersWithImagesAsync();
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
        [HttpGet("{id}/with-image")]
        public async Task<ActionResult<PlayerWithImageDto>> GetPlayerWithImage(int id)
        {
            var player = await _playerService.GetPlayerWithImageByIdAsync(id);
            if (player == null)
                return NotFound($"Player with ID {id} not found.");

            return Ok(player);
        }

        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayersByTeam(int teamId)
        {
            var players = await _playerService.GetPlayersByTeamIdAsync(teamId);
            return Ok(players);
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAvailablePlayers()
        {
            var players = await _playerService.GetAvailablePlayersAsync();
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

        [HttpPost]
        public async Task<ActionResult<PlayerDto>> CreatePlayer(CreatePlayerDto createPlayerDto)
        {
            try
            {
                var player = await _playerService.CreatePlayerAsync(createPlayerDto);
                return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating player: {ex.Message}");
            }
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
        [HttpPatch("{id}/enable")]
        public async Task<ActionResult> EnablePlayer(int id)
        {
            var result = await _playerService.EnablePlayerAsync(id);
            if (!result)
                return NotFound($"Player with ID {id} not found.");

            return NoContent();
        }
    }
}