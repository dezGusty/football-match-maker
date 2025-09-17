using FootballAPI.Models.Enums;
using FootballAPI.Models;

namespace FootballAPI.Repository
{
    public interface IMatchTeamsRepository
    {
        Task<IEnumerable<MatchTeams>> GetAllAsync();
        Task<MatchTeams> GetByIdAsync(int id);
        Task<MatchTeams> CreateAsync(MatchTeams matchTeam);
        Task<MatchTeams> UpdateAsync(MatchTeams matchTeam);
        Task<IEnumerable<MatchTeams>> GetByMatchIdAsync(int matchId);
        Task<MatchTeams> GetByMatchIdAndTeamIdAsync(int matchId, int teamId);
    }
}