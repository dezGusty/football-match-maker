using FootballAPI.DTOs;

namespace FootballAPI.Service.Interfaces
{
    public interface IMatchService
    {
        Task<IEnumerable<MatchDto>> GetAllMatchesAsync();
        Task<IEnumerable<PublicMatchDto>> GetPublicMatchesAsync();
        Task<IEnumerable<MatchDto>> GetMatchesByOrganiserAsync(int organiserId);
        Task<MatchDto?> GetMatchByIdAsync(int id);
        Task<MatchDto> CreateMatchAsync(CreateMatchDto createMatchDto, int organiserId);
        Task<MatchDto> UpdateMatchAsync(int id, UpdateMatchDto updateMatchDto, int organiserId);
        Task<bool> DeleteMatchAsync(int id, int organiserId);
        Task<MatchPlayerDto> JoinMatchAsync(JoinMatchDto joinMatchDto, int playerId);
        Task<bool> LeaveMatchAsync(int matchId, int playerId);
        Task<bool> UpdatePlayerTeamAsync(UpdatePlayerTeamDto updatePlayerTeamDto, int organiserId);
        Task<IEnumerable<TeamDto>> GetMatchTeamsAsync(int matchId);
    }
}