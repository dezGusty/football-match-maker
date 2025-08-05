using Microsoft.EntityFrameworkCore;
using FootballAPI.Data;
using FootballAPI.Models;

namespace FootballAPI.Repository
{
    public interface IPlayerRepository
    {
        Task<IEnumerable<Player>> GetAllAsync();
        Task<Player?> GetByIdAsync(int id);
        Task<IEnumerable<Player>> GetByTeamIdAsync(int teamId);
        Task<IEnumerable<Player>> GetAvailablePlayersAsync();
        Task<Player> CreateAsync(Player player);
        Task<Player> UpdateAsync(Player player);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Player>> SearchByNameAsync(string searchTerm);
        Task<IEnumerable<Player>> GetEnabledPlayersAsync();
        Task<IEnumerable<Player>> GetDisabledPlayersAsync();
        Task AddPlayerOrganiserRelationAsync(PlayerOrganiser relation);
    }
}