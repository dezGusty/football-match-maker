using FootballAPI.DTOs;

namespace FootballAPI.Service.Interfaces
{
    public interface IMatchHistoryService
    {
        Task<IEnumerable<MatchHistoryDto>> GetAllMatchHistoriesAsync();
        Task<MatchHistoryDto?> GetMatchHistoryByIdAsync(int id);
        Task<MatchHistoryDto?> GetMatchHistoryByMatchIdAsync(int matchId);
        Task<IEnumerable<MatchHistorySummaryDto>> GetPlayerMatchHistoriesAsync(int playerId);
        Task<IEnumerable<MatchHistorySummaryDto>> GetOrganiserMatchHistoriesAsync(int organiserId);
        Task<MatchHistoryDto> CreateMatchHistoryAsync(CreateMatchHistoryDto createMatchHistoryDto, int organiserId);
        Task<MatchHistoryDto> UpdateMatchHistoryAsync(int id, UpdateMatchHistoryDto updateMatchHistoryDto, int organiserId);
        Task<bool> DeleteMatchHistoryAsync(int id, int organiserId);
        Task<PlayerStatsAggregateDto> GetPlayerStatsAggregateAsync(int playerId);
    }
}