using FootballAPI.AppDbContext;
using FootballAPI.Models;
using FootballAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootballAPI.Repository
{
    public class MatchHistoryRepository : IMatchHistoryRepository
    {
        private readonly FootballDbContext _context;

        public MatchHistoryRepository(FootballDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MatchHistory>> GetAllMatchHistoriesAsync()
        {
            return await _context.MatchHistories
                .Include(mh => mh.Match)
                    .ThenInclude(m => m.Organiser)
                .Include(mh => mh.PlayerStats)
                    .ThenInclude(ps => ps.Player)
                .OrderByDescending(mh => mh.CompletedAt)
                .ToListAsync();
        }

        public async Task<MatchHistory?> GetMatchHistoryByIdAsync(int id)
        {
            return await _context.MatchHistories
                .Include(mh => mh.Match)
                    .ThenInclude(m => m.Organiser)
                .Include(mh => mh.PlayerStats)
                    .ThenInclude(ps => ps.Player)
                        .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(mh => mh.Id == id);
        }

        public async Task<MatchHistory?> GetMatchHistoryByMatchIdAsync(int matchId)
        {
            return await _context.MatchHistories
                .Include(mh => mh.Match)
                    .ThenInclude(m => m.Organiser)
                .Include(mh => mh.PlayerStats)
                    .ThenInclude(ps => ps.Player)
                        .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(mh => mh.MatchId == matchId);
        }

        public async Task<MatchHistory> CreateMatchHistoryAsync(MatchHistory matchHistory)
        {
            _context.MatchHistories.Add(matchHistory);
            await _context.SaveChangesAsync();
            return matchHistory;
        }

        public async Task<MatchHistory> UpdateMatchHistoryAsync(MatchHistory matchHistory)
        {
            _context.MatchHistories.Update(matchHistory);
            await _context.SaveChangesAsync();
            return matchHistory;
        }

        public async Task<bool> DeleteMatchHistoryAsync(int id)
        {
            var matchHistory = await _context.MatchHistories.FindAsync(id);
            if (matchHistory == null) return false;

            _context.MatchHistories.Remove(matchHistory);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MatchHistory>> GetPlayerMatchHistoriesAsync(int playerId)
        {
            return await _context.MatchHistories
                .Include(mh => mh.Match)
                    .ThenInclude(m => m.Organiser)
                .Include(mh => mh.PlayerStats)
                    .ThenInclude(ps => ps.Player)
                .Where(mh => mh.PlayerStats.Any(ps => ps.PlayerId == playerId))
                .OrderByDescending(mh => mh.CompletedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MatchHistory>> GetOrganiserMatchHistoriesAsync(int organiserId)
        {
            return await _context.MatchHistories
                .Include(mh => mh.Match)
                    .ThenInclude(m => m.Organiser)
                .Include(mh => mh.PlayerStats)
                    .ThenInclude(ps => ps.Player)
                .Where(mh => mh.Match.OrganiserId == organiserId)
                .OrderByDescending(mh => mh.CompletedAt)
                .ToListAsync();
        }
    }
}