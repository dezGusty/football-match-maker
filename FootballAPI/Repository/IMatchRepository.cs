using FootballAPI.Models;

namespace FootballAPI.Repository
{
    public interface IMatchRepository
    {
        Task<IEnumerable<Match>> GetAllAsync();
        Task<Match> GetByIdAsync(int id);
        Task<Match> CreateAsync(Match match);
        Task<Match> UpdateAsync(Match match);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Match>> GetMatchesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Match>> GetMatchesByTeamIdAsync(int teamId);
        Task<IEnumerable<Match>> GetPastMatchesAsync();
        Task<IEnumerable<Match>> GetFutureMatchesAsync();
    }
}