using FootballAPI.AppDbContext;
using FootballAPI.Models;
using FootballAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootballAPI.Repository
{
    public class MatchPlayerRepository : IMatchPlayerRepository
    {
        private readonly FootballDbContext _context;

        public MatchPlayerRepository(FootballDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MatchPlayer>> GetMatchPlayersAsync(int matchId)
        {
            return await _context.MatchPlayers
                .Include(mp => mp.Player)
                    .ThenInclude(p => p.User)
                .Include(mp => mp.Match)
                .Where(mp => mp.MatchId == matchId)
                .OrderBy(mp => mp.TeamNumber)
                .ThenBy(mp => mp.JoinedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MatchPlayer>> GetPlayerMatchesAsync(int playerId)
        {
            return await _context.MatchPlayers
                .Include(mp => mp.Match)
                    .ThenInclude(m => m.Organiser)
                .Where(mp => mp.PlayerId == playerId)
                .OrderByDescending(mp => mp.Match.DateTime)
                .ToListAsync();
        }

        public async Task<MatchPlayer?> GetMatchPlayerAsync(int matchId, int playerId)
        {
            return await _context.MatchPlayers
                .Include(mp => mp.Player)
                    .ThenInclude(p => p.User)
                .Include(mp => mp.Match)
                .FirstOrDefaultAsync(mp => mp.MatchId == matchId && mp.PlayerId == playerId);
        }

        public async Task<MatchPlayer> AddPlayerToMatchAsync(MatchPlayer matchPlayer)
        {
            _context.MatchPlayers.Add(matchPlayer);
            await _context.SaveChangesAsync();
            return matchPlayer;
        }

        public async Task<bool> RemovePlayerFromMatchAsync(int matchId, int playerId)
        {
            var matchPlayer = await _context.MatchPlayers
                .FirstOrDefaultAsync(mp => mp.MatchId == matchId && mp.PlayerId == playerId);
            
            if (matchPlayer == null) return false;

            _context.MatchPlayers.Remove(matchPlayer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePlayerTeamAsync(int matchId, int playerId, int teamNumber)
        {
            var matchPlayer = await _context.MatchPlayers
                .FirstOrDefaultAsync(mp => mp.MatchId == matchId && mp.PlayerId == playerId);
            
            if (matchPlayer == null) return false;

            matchPlayer.TeamNumber = teamNumber;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsPlayerInMatchAsync(int matchId, int playerId)
        {
            return await _context.MatchPlayers
                .AnyAsync(mp => mp.MatchId == matchId && mp.PlayerId == playerId);
        }

        public async Task<int> GetTeamPlayerCountAsync(int matchId, int teamNumber)
        {
            return await _context.MatchPlayers
                .CountAsync(mp => mp.MatchId == matchId && mp.TeamNumber == teamNumber);
        }

        public async Task<IEnumerable<MatchPlayer>> GetTeamPlayersAsync(int matchId, int teamNumber)
        {
            return await _context.MatchPlayers
                .Include(mp => mp.Player)
                    .ThenInclude(p => p.User)
                .Where(mp => mp.MatchId == matchId && mp.TeamNumber == teamNumber)
                .OrderBy(mp => mp.JoinedAt)
                .ToListAsync();
        }
    }
}