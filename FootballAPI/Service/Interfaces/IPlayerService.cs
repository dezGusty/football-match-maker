using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository;
namespace FootballAPI.Service
{
    public interface IPlayerService
    {
        Task<IEnumerable<PlayerDto>> GetAllPlayersAsync();
        Task<PlayerDto?> GetPlayerByIdAsync(int id);
        Task<IEnumerable<PlayerDto>> GetAvailablePlayersAsync();

        Task<IEnumerable<PlayerDto>> GetAvailablePlayersByOrganiserAsync(int organiserId);
        Task<PlayerDto?> UpdatePlayerAsync(int id, UpdatePlayerDto updatePlayerDto);
        Task<bool> DeletePlayerAsync(int id);
        Task<IEnumerable<PlayerDto>> SearchPlayersByNameAsync(string searchTerm);
        Task<bool> PlayerExistsAsync(int id);
        Task<bool> EnablePlayerAsync(int id);
        Task<bool> SetPlayerAvailableAsync(int playerId);
        Task<bool> SetPlayerUnavailableAsync(int playerId);
        Task<bool> SetMultiplePlayersAvailableAsync(int[] playerIds);
        Task<bool> SetMultiplePlayersUnavailableAsync(int[] playerIds);
        Task<bool> ClearAllAvailablePlayersAsync();
        Task<bool> UpdatePlayerRatingAsync(int playerId, float ratingChange);
        Task<bool> UpdateMultiplePlayerRatingsAsync(List<PlayerRatingUpdateDto> playerRatingUpdates);
        Task AddPlayerOrganiserRelationAsync(int playerId, int organiserId);
        Task<string> UpdatePlayerProfileImageAsync(int playerId, IFormFile imageFile);
        Task<bool> DeletePlayerProfileImageAsync(int playerId);
    }
}