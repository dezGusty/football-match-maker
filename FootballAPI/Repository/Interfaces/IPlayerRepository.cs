using FootballAPI.Models.Enums;
using Microsoft.EntityFrameworkCore;
using FootballAPI.Data;
using FootballAPI.Models;

namespace FootballAPI.Repository
{
    public interface IPlayerRepository
    {
        Task<IEnumerable<Player>> GetAllAsync();
        Task<Player?> GetByIdAsync(int id);
        Task<Player?> GetByUserIdAsync(int userId);
        Task<IEnumerable<Player>> GetAvailablePlayersAsync();
        Task<Player> CreateAsync(Player player);
        Task<Player> UpdateAsync(Player player);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Player>> SearchByNameAsync(string searchTerm);
        Task<IEnumerable<Player>> GetActivePlayersAsync();
        Task<IEnumerable<Player>> GetDeletedPlayersAsync();
        Task<bool> HardDeleteAsync(int id);
        Task AddPlayerOrganiserRelationAsync(PlayerOrganiser relation);
    }
}