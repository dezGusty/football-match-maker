using FootballAPI.Models;

namespace FootballAPI.Repository.Interfaces
{
    public interface IPlayerMatchStatsRepository
    {
        Task<IEnumerable<PlayerMatchStats>> GetPlayerStatsAsync(int playerId);
        Task<IEnumerable<PlayerMatchStats>> GetMatchStatsAsync(int matchHistoryId);
        Task<PlayerMatchStats?> GetPlayerMatchStatAsync(int matchHistoryId, int playerId);
        Task<PlayerMatchStats> CreatePlayerMatchStatAsync(PlayerMatchStats playerMatchStats);
        Task<PlayerMatchStats> UpdatePlayerMatchStatAsync(PlayerMatchStats playerMatchStats);
        Task<bool> DeletePlayerMatchStatAsync(int id);
        Task<Dictionary<int, int>> GetPlayerGoalStatsAsync(int playerId);
        Task<Dictionary<int, int>> GetPlayerAssistStatsAsync(int playerId);
        Task<float> GetPlayerAverageRatingAsync(int playerId);
    }
}