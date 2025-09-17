using FootballAPI.Models.Enums;
using FootballAPI.Models;

namespace FootballAPI.Repository
{
    public interface ITeamPlayersRepository
    {
        Task<TeamPlayers> CreateAsync(TeamPlayers teamPlayer);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<TeamPlayers>> GetByMatchTeamIdAsync(int matchTeamId);
        Task<TeamPlayers> GetByMatchTeamIdAndUserIdAsync(int matchTeamId, int userId);
    }
}