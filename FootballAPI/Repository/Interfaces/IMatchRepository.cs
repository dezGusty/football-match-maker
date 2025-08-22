using FootballAPI.Models;

namespace FootballAPI.Repository.Interfaces
{
    public interface IMatchRepository
    {
        Task<IEnumerable<Match>> GetAllMatchesAsync();
        Task<IEnumerable<Match>> GetPublicMatchesAsync();
        Task<IEnumerable<Match>> GetMatchesByOrganiserAsync(int organiserId);
        Task<Match?> GetMatchByIdAsync(int id);
        Task<Match> CreateMatchAsync(Match match);
        Task<Match> UpdateMatchAsync(Match match);
        Task<bool> DeleteMatchAsync(int id);
        Task<bool> MatchExistsAsync(int id);
        Task<int> GetMatchPlayerCountAsync(int matchId);
        Task<bool> IsMatchFullAsync(int matchId);
        Task<IEnumerable<Match>> GetMatchesByPlayerAsync(int playerId);
    }
}