using FootballAPI.AppDbContext;
using FootballAPI.Models;
using FootballAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootballAPI.Repository
{
    public class PlayerMatchStatsRepository : IPlayerMatchStatsRepository
    {
        private readonly FootballDbContext _context;

        public PlayerMatchStatsRepository(FootballDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PlayerMatchStats>> GetPlayerStatsAsync(int playerId)
        {
            return await _context.PlayerMatchStats
                .Include(pms => pms.MatchHistory)
                    .ThenInclude(mh => mh.Match)
                .Include(pms => pms.Player)
                .Where(pms => pms.PlayerId == playerId)
                .OrderByDescending(pms => pms.MatchHistory.CompletedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerMatchStats>> GetMatchStatsAsync(int matchHistoryId)
        {
            return await _context.PlayerMatchStats
                .Include(pms => pms.Player)
                    .ThenInclude(p => p.User)
                .Include(pms => pms.MatchHistory)
                .Where(pms => pms.MatchHistoryId == matchHistoryId)
                .OrderBy(pms => pms.TeamNumber)
                .ThenByDescending(pms => pms.Goals)
                .ToListAsync();
        }

        public async Task<PlayerMatchStats?> GetPlayerMatchStatAsync(int matchHistoryId, int playerId)
        {
            return await _context.PlayerMatchStats
                .Include(pms => pms.Player)
                    .ThenInclude(p => p.User)
                .Include(pms => pms.MatchHistory)
                    .ThenInclude(mh => mh.Match)
                .FirstOrDefaultAsync(pms => pms.MatchHistoryId == matchHistoryId && pms.PlayerId == playerId);
        }

        public async Task<PlayerMatchStats> CreatePlayerMatchStatAsync(PlayerMatchStats playerMatchStats)
        {
            _context.PlayerMatchStats.Add(playerMatchStats);
            await _context.SaveChangesAsync();
            return playerMatchStats;
        }

        public async Task<PlayerMatchStats> UpdatePlayerMatchStatAsync(PlayerMatchStats playerMatchStats)
        {
            _context.PlayerMatchStats.Update(playerMatchStats);
            await _context.SaveChangesAsync();
            return playerMatchStats;
        }

        public async Task<bool> DeletePlayerMatchStatAsync(int id)
        {
            var playerMatchStats = await _context.PlayerMatchStats.FindAsync(id);
            if (playerMatchStats == null) return false;

            _context.PlayerMatchStats.Remove(playerMatchStats);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<int, int>> GetPlayerGoalStatsAsync(int playerId)
        {
            return await _context.PlayerMatchStats
                .Where(pms => pms.PlayerId == playerId)
                .GroupBy(pms => pms.MatchHistoryId)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(pms => pms.Goals));
        }

        public async Task<Dictionary<int, int>> GetPlayerAssistStatsAsync(int playerId)
        {
            return await _context.PlayerMatchStats
                .Where(pms => pms.PlayerId == playerId)
                .GroupBy(pms => pms.MatchHistoryId)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(pms => pms.Assists));
        }

        public async Task<float> GetPlayerAverageRatingAsync(int playerId)
        {
            var ratings = await _context.PlayerMatchStats
                .Where(pms => pms.PlayerId == playerId && pms.Rating.HasValue)
                .Select(pms => pms.Rating!.Value)
                .ToListAsync();

            return ratings.Any() ? ratings.Average() : 0.0f;
        }
    }
}