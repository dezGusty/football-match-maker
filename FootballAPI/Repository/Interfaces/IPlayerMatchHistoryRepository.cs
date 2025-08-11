using FootballAPI.Models;

namespace FootballAPI.Repository
{
    public interface IPlayerMatchHistoryRepository
    {
        Task<IEnumerable<PlayerMatchHistory>> GetAllAsync();
        Task<PlayerMatchHistory> GetByIdAsync(int id);
        Task<PlayerMatchHistory> CreateAsync(PlayerMatchHistory playerMatchHistory);
        Task<PlayerMatchHistory> UpdateAsync(PlayerMatchHistory playerMatchHistory);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<PlayerMatchHistory>> GetByPlayerIdAsync(int playerId);
        Task<IEnumerable<PlayerMatchHistory>> GetByTeamIdAsync(int teamId);
        Task<IEnumerable<PlayerMatchHistory>> GetByMatchIdAsync(int matchId);
        Task<float> GetAveragePerformanceRatingAsync(int playerId);
        Task<IEnumerable<PlayerMatchHistory>> GetTopPerformancesAsync(int count = 10);
    }
}