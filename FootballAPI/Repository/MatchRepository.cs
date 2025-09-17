using Microsoft.EntityFrameworkCore;
using FootballAPI.Data;
using FootballAPI.Models;
using FootballAPI.Models.Enums;

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
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();
        }

        public async Task<Match> GetByIdAsync(int id)
        {
            return await _context.Matches.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Match>> GetMyPublicMatchesAsync(int id)
        {
            return await _context.Matches
                .Where(m => m.IsPublic &&
                           m.Status == Status.Open &&
                           m.OrganiserId != id &&
                           !m.MatchTeams.Any(mt => mt.TeamPlayers.Any(tp => tp.UserId == id)))
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<Match>> GetPublicMatchesAsync()
        {
            return await _context.Matches
                .Where(m => m.IsPublic && m.Status == Status.Open)
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

        public async Task<IEnumerable<Match>> GetPastMatchesAsync(int id)
        {
            var currentDate = DateTime.Now;
            return await _context.Matches
                .Where(m => m.MatchDate <= currentDate && (m.Status == Status.Closed || m.Status == Status.Finalized) && m.OrganiserId == id)
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<Match>> GetPastMatchesByParticipantAsync(int userId)
        {
            var currentDate = DateTime.Now;
            return await _context.Matches
                .Where(m => m.MatchDate <= currentDate && (m.Status == Status.Closed || m.Status == Status.Finalized) && m.MatchTeams.Any(mt => mt.TeamPlayers.Any(p => p.UserId == userId)))
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> GetFutureMatchesAsync()
        {
            var currentDate = DateTime.Now;
            return await _context.Matches
                .Where(m => m.MatchDate.Date > currentDate)
                .OrderBy(m => m.MatchDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> GetMatchesByOrganiserAsync(int organiserId)
        {
            return await _context.Matches
                .Where(m => m.OrganiserId == organiserId)
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();
        }



    }
}