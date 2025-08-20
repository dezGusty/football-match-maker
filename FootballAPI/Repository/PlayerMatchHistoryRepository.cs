using Microsoft.EntityFrameworkCore;
using FootballAPI.Data;
using FootballAPI.Models;

namespace FootballAPI.Repository
{
    public class PlayerMatchHistoryRepository : IPlayerMatchHistoryRepository
    {
        private readonly FootballDbContext _context;

        public PlayerMatchHistoryRepository(FootballDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PlayerMatchHistory>> GetAllAsync()
        {
            return await _context.Set<PlayerMatchHistory>()
                .Include(pmh => pmh.Player)
                .Include(pmh => pmh.Team)
                .Include(pmh => pmh.Match)
                .OrderByDescending(pmh => pmh.RecordDate)
                .ToListAsync();
        }

        public async Task<PlayerMatchHistory> GetByIdAsync(int id)
        {
            return await _context.Set<PlayerMatchHistory>()
                .Include(pmh => pmh.Player)
                .Include(pmh => pmh.Team)
                .Include(pmh => pmh.Match)
                .FirstOrDefaultAsync(pmh => pmh.Id == id);
        }

        public async Task<IEnumerable<PlayerMatchHistory>> GetByPlayerIdAsync(int playerId)
        {
            return await _context.Set<PlayerMatchHistory>()
                .Include(pmh => pmh.Player)
                .Include(pmh => pmh.Team)
                .Include(pmh => pmh.Match)
                .Where(pmh => pmh.PlayerId == playerId)
                .OrderByDescending(pmh => pmh.RecordDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerMatchHistory>> GetByTeamIdAsync(int teamId)
        {
            return await _context.Set<PlayerMatchHistory>()
                .Include(pmh => pmh.Player)
                .Include(pmh => pmh.Team)
                .Include(pmh => pmh.Match)
                .Where(pmh => pmh.TeamId == teamId)
                .OrderByDescending(pmh => pmh.RecordDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerMatchHistory>> GetByMatchIdAsync(int matchId)
        {
            return await _context.Set<PlayerMatchHistory>()
                .Include(pmh => pmh.Player)
                .Include(pmh => pmh.Team)
                .Include(pmh => pmh.Match)
                .Where(pmh => pmh.MatchId == matchId)
                .OrderByDescending(pmh => pmh.PerformanceRating)
                .ToListAsync();
        }

        public async Task<float> GetAveragePerformanceRatingAsync(int playerId)
        {
            var ratings = await _context.Set<PlayerMatchHistory>()
                .Where(pmh => pmh.PlayerId == playerId)
                .Select(pmh => pmh.PerformanceRating)
                .ToListAsync();

            return ratings.Any() ? ratings.Average() : 0.0f;
        }

        public async Task<IEnumerable<PlayerMatchHistory>> GetTopPerformancesAsync(int count = 10)
        {
            return await _context.Set<PlayerMatchHistory>()
                .Include(pmh => pmh.Player)
                .Include(pmh => pmh.Team)
                .Include(pmh => pmh.Match)
                .OrderByDescending(pmh => pmh.PerformanceRating)
                .Take(count)
                .ToListAsync();
        }

        public async Task<PlayerMatchHistory> CreateAsync(PlayerMatchHistory playerMatchHistory)
        {
            playerMatchHistory.RecordDate = DateTime.Now;
            _context.Set<PlayerMatchHistory>().Add(playerMatchHistory);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(playerMatchHistory.Id);
        }

        public async Task<PlayerMatchHistory> UpdateAsync(PlayerMatchHistory playerMatchHistory)
        {
            _context.Entry(playerMatchHistory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return await GetByIdAsync(playerMatchHistory.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var playerMatchHistory = await _context.Set<PlayerMatchHistory>().FindAsync(id);
            if (playerMatchHistory == null)
                return false;

            _context.Set<PlayerMatchHistory>().Remove(playerMatchHistory);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Set<PlayerMatchHistory>().AnyAsync(pmh => pmh.Id == id);
        }
    }
}