using FootballAPI.Models.Enums;
using FootballAPI.DTOs;

namespace FootballAPI.Service
{
    public interface IMatchTeamsService
    {
        Task<IEnumerable<MatchTeamsDto>> GetAllMatchTeamsAsync();
        Task<MatchTeamsDto> GetMatchTeamByIdAsync(int id);
        Task<MatchTeamsDto> CreateMatchTeamAsync(CreateMatchTeamsDto createMatchTeamsDto);
        Task<MatchTeamsDto> UpdateMatchTeamAsync(int id, UpdateMatchTeamsDto updateMatchTeamsDto);
        Task<IEnumerable<MatchTeamsDto>> GetMatchTeamsByMatchIdAsync(int matchId);
        Task<MatchTeamsDto> GetMatchTeamByMatchIdAndTeamIdAsync(int matchId, int teamId);
    }
}