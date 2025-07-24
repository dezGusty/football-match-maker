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
        [HttpPatch("{id}/set-available")]
        public async Task<ActionResult> SetPlayerAvailable(int id)
        {
            try
            {
                var result = await _playerService.SetPlayerAvailableAsync(id);
                if (!result)
                    return NotFound($"Player with ID {id} not found or is disabled.");

                return Ok(new { message = "Player set as available successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error setting player available: {ex.Message}");
            }
        }

        [HttpPatch("{id}/set-unavailable")]
        public async Task<ActionResult> SetPlayerUnavailable(int id)
        {
            try
            {
                var result = await _playerService.SetPlayerUnavailableAsync(id);
                if (!result)
                    return NotFound($"Player with ID {id} not found.");

                return Ok(new { message = "Player set as unavailable successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error setting player unavailable: {ex.Message}");
            }
        }

        [HttpPatch("set-multiple-available")]
        public async Task<ActionResult> SetMultiplePlayersAvailable([FromBody] int[] playerIds)
        {
            try
            {
                if (playerIds == null || playerIds.Length == 0)
                    return BadRequest("Player IDs array cannot be empty.");

                var result = await _playerService.SetMultiplePlayersAvailableAsync(playerIds);
                if (!result)
                    return BadRequest("Failed to set players as available.");

                return Ok(new { message = $"{playerIds.Length} players set as available successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error setting multiple players available: {ex.Message}");
            }
        }

        [HttpPatch("set-multiple-unavailable")]
        public async Task<ActionResult> SetMultiplePlayersUnavailable([FromBody] int[] playerIds)
        {
            try
            {
                if (playerIds == null || playerIds.Length == 0)
                    return BadRequest("Player IDs array cannot be empty.");

                var result = await _playerService.SetMultiplePlayersUnavailableAsync(playerIds);
                if (!result)
                    return BadRequest("Failed to set players as unavailable.");

                return Ok(new { message = $"{playerIds.Length} players set as unavailable successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error setting multiple players unavailable: {ex.Message}");
            }
        }

        [HttpPatch("clear-all-available")]
        public async Task<ActionResult> ClearAllAvailablePlayers()
        {
            try
            {
                var result = await _playerService.ClearAllAvailablePlayersAsync();
                if (!result)
                    return BadRequest("Failed to clear available players.");

                return Ok(new { message = "All players set as unavailable successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error clearing available players: {ex.Message}");
            }
        }
    }
}