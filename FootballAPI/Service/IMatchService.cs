using FootballAPI.DTOs;

namespace FootballAPI.Service
{
    public interface IMatchService
    {
        Task<IEnumerable<MatchDto>> GetAllMatchesAsync();
        Task<MatchDto> GetMatchByIdAsync(int id);
        
        Task<MatchDto> CreateMatchAsync(CreateMatchDto createMatchDto);
        Task<MatchDto> UpdateMatchAsync(int id, UpdateMatchDto updateMatchDto);
        Task<bool> DeleteMatchAsync(int id);
        Task<IEnumerable<MatchDto>> GetMatchesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<MatchDto>> GetMatchesByTeamIdAsync(int teamId);
    }
}