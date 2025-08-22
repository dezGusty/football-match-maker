using FootballAPI.Models;

namespace FootballAPI.Repository.Interfaces
{
    public interface IMatchHistoryRepository
    {
        Task<IEnumerable<MatchHistory>> GetAllMatchHistoriesAsync();
        Task<MatchHistory?> GetMatchHistoryByIdAsync(int id);
        Task<MatchHistory?> GetMatchHistoryByMatchIdAsync(int matchId);
        Task<MatchHistory> CreateMatchHistoryAsync(MatchHistory matchHistory);
        Task<MatchHistory> UpdateMatchHistoryAsync(MatchHistory matchHistory);
        Task<bool> DeleteMatchHistoryAsync(int id);
        Task<IEnumerable<MatchHistory>> GetPlayerMatchHistoriesAsync(int playerId);
        Task<IEnumerable<MatchHistory>> GetOrganiserMatchHistoriesAsync(int organiserId);
    }
}