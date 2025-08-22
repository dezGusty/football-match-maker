using FootballAPI.AppDbContext;
using FootballAPI.Models;
using FootballAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootballAPI.Repository
{
    public class MatchRepository : IMatchRepository
    {
        private readonly FootballDbContext _context;

        public MatchRepository(FootballDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Match>> GetAllMatchesAsync()
        {
            return await _context.Matches
                .Include(m => m.Organiser)
                .Include(m => m.MatchPlayers)
                    .ThenInclude(mp => mp.Player)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> GetPublicMatchesAsync()
        {
            return await _context.Matches
                .Include(m => m.Organiser)
                .Include(m => m.MatchPlayers)
                    .ThenInclude(mp => mp.Player)
                .Where(m => m.IsPublic && m.Status == MatchStatus.Scheduled)
                .OrderBy(m => m.DateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> GetMatchesByOrganiserAsync(int organiserId)
        {
            return await _context.Matches
                .Include(m => m.Organiser)
                .Include(m => m.MatchPlayers)
                    .ThenInclude(mp => mp.Player)
                .Where(m => m.OrganiserId == organiserId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<Match?> GetMatchByIdAsync(int id)
        {
            return await _context.Matches
                .Include(m => m.Organiser)
                .Include(m => m.MatchPlayers)
                    .ThenInclude(mp => mp.Player)
                .Include(m => m.MatchHistory)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Match> CreateMatchAsync(Match match)
        {
            _context.Matches.Add(match);
            await _context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> UpdateMatchAsync(Match match)
        {
            _context.Matches.Update(match);
            await _context.SaveChangesAsync();
            return match;
        }

        public async Task<bool> DeleteMatchAsync(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null) return false;

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MatchExistsAsync(int id)
        {
            return await _context.Matches.AnyAsync(m => m.Id == id);
        }

        public async Task<int> GetMatchPlayerCountAsync(int matchId)
        {
            return await _context.MatchPlayers.CountAsync(mp => mp.MatchId == matchId);
        }

        public async Task<bool> IsMatchFullAsync(int matchId)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null) return false;

            var playerCount = await GetMatchPlayerCountAsync(matchId);
            return playerCount >= match.MaxPlayers;
        }

        public async Task<IEnumerable<Match>> GetMatchesByPlayerAsync(int playerId)
        {
            return await _context.Matches
                .Include(m => m.Organiser)
                .Include(m => m.MatchPlayers)
                    .ThenInclude(mp => mp.Player)
                .Where(m => m.MatchPlayers.Any(mp => mp.PlayerId == playerId))
                .OrderByDescending(m => m.DateTime)
                .ToListAsync();
        }
    }
}