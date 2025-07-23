using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository;
namespace FootballAPI.Service
{
    public interface IPlayerService
    {
        Task<IEnumerable<PlayerDto>> GetAllPlayersAsync();
        Task<PlayerDto?> GetPlayerByIdAsync(int id);
        Task<PlayerWithImageDto?> GetPlayerWithImageByIdAsync(int id);
        Task<IEnumerable<PlayerDto>> GetPlayersByTeamIdAsync(int teamId);
        Task<IEnumerable<PlayerDto>> GetAvailablePlayersAsync();
        Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto createPlayerDto);
        Task<PlayerDto?> UpdatePlayerAsync(int id, UpdatePlayerDto updatePlayerDto);
        Task<bool> DeletePlayerAsync(int id);
        Task<IEnumerable<PlayerDto>> SearchPlayersByNameAsync(string searchTerm);
        Task<bool> PlayerExistsAsync(int id);
        Task<bool> EnablePlayerAsync(int id);
    }
}