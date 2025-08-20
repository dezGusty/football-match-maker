using FootballAPI.DTOs;
using FootballAPI.Repository;

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
        Task<IEnumerable<MatchDto>> GetPublicMatchesAsync();
        Task<IEnumerable<MatchDto>> GetMatchesByStatusAsync(Status status);
        Task<IEnumerable<MatchDto>> GetFutureMatchesAsync();
        Task<IEnumerable<MatchDto>> GetPastMatchesAsync();
    }
}