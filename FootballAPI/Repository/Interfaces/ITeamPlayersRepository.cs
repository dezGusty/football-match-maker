using FootballAPI.Models.Enums;
using FootballAPI.Models;

namespace FootballAPI.Repository
{
    public interface ITeamPlayersRepository
    {
        Task<IEnumerable<TeamPlayers>> GetAllAsync();
        Task<TeamPlayers> GetByIdAsync(int id);
        Task<TeamPlayers> CreateAsync(TeamPlayers teamPlayer);
        Task<TeamPlayers> UpdateAsync(TeamPlayers teamPlayer);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<TeamPlayers>> GetByMatchTeamIdAsync(int matchTeamId);
        Task<IEnumerable<TeamPlayers>> GetByUserIdAsync(int userId);
        Task<TeamPlayers> GetByMatchTeamIdAndUserIdAsync(int matchTeamId, int userId);
        Task<IEnumerable<TeamPlayers>> GetByStatusAsync(PlayerStatus status);
    }
}