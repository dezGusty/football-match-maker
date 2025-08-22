using FootballAPI.Models;

namespace FootballAPI.Repository.Interfaces
{
    public interface IMatchPlayerRepository
    {
        Task<IEnumerable<MatchPlayer>> GetMatchPlayersAsync(int matchId);
        Task<IEnumerable<MatchPlayer>> GetPlayerMatchesAsync(int playerId);
        Task<MatchPlayer?> GetMatchPlayerAsync(int matchId, int playerId);
        Task<MatchPlayer> AddPlayerToMatchAsync(MatchPlayer matchPlayer);
        Task<bool> RemovePlayerFromMatchAsync(int matchId, int playerId);
        Task<bool> UpdatePlayerTeamAsync(int matchId, int playerId, int teamNumber);
        Task<bool> IsPlayerInMatchAsync(int matchId, int playerId);
        Task<int> GetTeamPlayerCountAsync(int matchId, int teamNumber);
        Task<IEnumerable<MatchPlayer>> GetTeamPlayersAsync(int matchId, int teamNumber);
    }
}