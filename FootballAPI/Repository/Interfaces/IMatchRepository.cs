using FootballAPI.Models.Enums;
using FootballAPI.Models;
using FootballAPI.Models.Enums;

namespace FootballAPI.Repository
{
    public interface IMatchRepository
    {
        Task<IEnumerable<Match>> GetAllAsync();
        Task<Match> GetByIdAsync(int id);
        Task<Match> CreateAsync(Match match);
        Task<Match> UpdateAsync(Match match);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Match>> GetPublicMatchesAsync();
        Task<IEnumerable<Match>> GetPastMatchesAsync(int id);
        Task<IEnumerable<Match>> GetPastMatchesByParticipantAsync(int userId);
        Task<IEnumerable<Match>> GetFutureMatchesAsync();
        Task<IEnumerable<Match>> GetMatchesByOrganiserAsync(int organiserId);
        Task<IEnumerable<Match>> GetMyPublicMatchesAsync(int id);
    }
}