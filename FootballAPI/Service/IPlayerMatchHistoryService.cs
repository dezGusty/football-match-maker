using FootballAPI.DTOs;
using FootballAPI.Models;

namespace FootballAPI.Service
{
    public interface IPlayerMatchHistoryService
    {
        Task<IEnumerable<PlayerMatchHistoryDto>> GetAllPlayerMatchHistoryAsync();
        Task<PlayerMatchHistoryDto> GetPlayerMatchHistoryByIdAsync(int id);
        Task<IEnumerable<PlayerMatchHistoryDto>> GetPlayerMatchHistoryByPlayerIdAsync(int playerId);
        Task<IEnumerable<PlayerMatchHistoryDto>> GetPlayerMatchHistoryByTeamIdAsync(int teamId);
        Task<IEnumerable<PlayerMatchHistoryDto>> GetPlayerMatchHistoryByMatchIdAsync(int matchId);
        Task<PlayerMatchHistoryDto> CreatePlayerMatchHistoryAsync(PlayerMatchHistory playerMatchHistory);
        Task<PlayerMatchHistoryDto> UpdatePlayerMatchHistoryAsync(int id, PlayerMatchHistory playerMatchHistory);
        Task<bool> DeletePlayerMatchHistoryAsync(int id);
        Task<float> GetAveragePerformanceRatingAsync(int playerId);
        Task<IEnumerable<PlayerMatchHistoryDto>> GetTopPerformancesAsync(int count = 10);
    }
}