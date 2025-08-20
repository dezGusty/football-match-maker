using Microsoft.EntityFrameworkCore;
using FootballAPI.Data;
using FootballAPI.Models;

namespace FootballAPI.Repository
{
    public class MatchRepository : IMatchRepository
    {
        private readonly FootballDbContext _context;

        public MatchRepository(FootballDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Match>> GetAllAsync()
        {
            return await _context.Matches
                .Include(m => m.PlayerHistory)
                    .ThenInclude(ph => ph.Player)
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();
        }

        public async Task<Match> GetByIdAsync(int id)
        {
            return await _context.Matches
                .Include(m => m.PlayerHistory)
                    .ThenInclude(ph => ph.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Match>> GetMatchesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Matches
                .Include(m => m.PlayerHistory)
                    .ThenInclude(ph => ph.Player)
                .Where(m => m.MatchDate >= startDate && m.MatchDate <= endDate)
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> GetPublicMatchesAsync()
        {
            return await _context.Matches
                .Include(m => m.PlayerHistory)
                    .ThenInclude(ph => ph.Player)
                .Where(m => m.IsPublic)
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> GetMatchesByStatusAsync(Status status)
        {
            return await _context.Matches
                .Include(m => m.PlayerHistory)
                    .ThenInclude(ph => ph.Player)
                .Where(m => m.Status == status)
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();
        }

        public async Task<Match> CreateAsync(Match match)
        {
            _context.Matches.Add(match);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(match.Id);
        }

        public async Task<Match> UpdateAsync(Match match)
        {
            _context.Entry(match).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return await GetByIdAsync(match.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null)
                return false;

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Matches.AnyAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Match>> GetPastMatchesAsync()
        {
            var currentDate = DateTime.Now.Date;
            return await _context.Matches
                .Include(m => m.PlayerHistory)
                    .ThenInclude(ph => ph.Player)
                .Where(m => m.MatchDate.Date <= currentDate)
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();
        }


        public async Task<IEnumerable<Match>> GetFutureMatchesAsync()
        {
            var currentDate = DateTime.Now.Date;
            return await _context.Matches
                .Include(m => m.PlayerHistory)
                    .ThenInclude(ph => ph.Player)
                .Where(m => m.MatchDate.Date > currentDate)
                .OrderBy(m => m.MatchDate)
                .ToListAsync();
        }

    }
}