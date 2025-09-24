using Microsoft.EntityFrameworkCore;
using FootballAPI.Data;
using FootballAPI.Models;
using FootballAPI.DTOs;
using FootballAPI.Repository.Interfaces;

namespace FootballAPI.Repository
{
    public class RatingHistoryRepository : IRatingHistoryRepository
    {
        private readonly FootballDbContext _context;

        public RatingHistoryRepository(FootballDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RatingHistory>> GetAllAsync()
        {
            return await _context.RatingHistories
                .Include(rh => rh.User)
                .Include(rh => rh.Match)
                .OrderByDescending(rh => rh.CreatedAt)
                .ToListAsync();
        }

        public async Task<RatingHistory?> GetByIdAsync(int id)
        {
            return await _context.RatingHistories
                .Include(rh => rh.User)
                .Include(rh => rh.Match)
                .FirstOrDefaultAsync(rh => rh.Id == id);
        }

        public async Task<IEnumerable<RatingHistory>> GetByUserIdAsync(int userId, GetRatingHistoryDto? filters = null)
        {
            var query = _context.RatingHistories
                .Include(rh => rh.User)
                .Include(rh => rh.Match)
                .Where(rh => rh.UserId == userId);

            if (filters != null)
            {
                if (filters.MatchId.HasValue)
                    query = query.Where(rh => rh.MatchId == filters.MatchId);

                if (!string.IsNullOrEmpty(filters.ChangeReason))
                    query = query.Where(rh => rh.ChangeReason == filters.ChangeReason);

                if (filters.FromDate.HasValue)
                    query = query.Where(rh => rh.CreatedAt >= filters.FromDate);

                if (filters.ToDate.HasValue)
                    query = query.Where(rh => rh.CreatedAt <= filters.ToDate);
            }

            query = query.OrderByDescending(rh => rh.CreatedAt);

            if (filters != null)
            {
                query = query.Skip((filters.Page - 1) * filters.PageSize)
                           .Take(filters.PageSize);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<RatingHistory>> GetByMatchIdAsync(int matchId)
        {
            return await _context.RatingHistories
                .Include(rh => rh.User)
                .Include(rh => rh.Match)
                .Where(rh => rh.MatchId == matchId)
                .OrderByDescending(rh => rh.CreatedAt)
                .ToListAsync();
        }

        public async Task<RatingHistory> CreateAsync(RatingHistory ratingHistory)
        {
            _context.RatingHistories.Add(ratingHistory);
            await _context.SaveChangesAsync();

            // Load navigation properties
            return await GetByIdAsync(ratingHistory.Id) ?? ratingHistory;
        }

        public async Task<RatingHistory> UpdateAsync(RatingHistory ratingHistory)
        {
            _context.Entry(ratingHistory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ratingHistory;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ratingHistory = await _context.RatingHistories.FindAsync(id);
            if (ratingHistory == null)
                return false;

            _context.RatingHistories.Remove(ratingHistory);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<RatingStatisticsDto> GetUserRatingStatisticsAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found", nameof(userId));

            var ratingHistory = await _context.RatingHistories
                .Where(rh => rh.UserId == userId)
                .OrderBy(rh => rh.CreatedAt)
                .ToListAsync();

            if (!ratingHistory.Any())
            {
                return new RatingStatisticsDto
                {
                    UserId = userId,
                    UserName = $"{user.FirstName} {user.LastName}",
                    CurrentRating = user.Rating,
                    StartingRating = user.Rating,
                    HighestRating = user.Rating,
                    LowestRating = user.Rating,
                    TotalRatingChanges = 0,
                    MatchesPlayed = 0,
                    ManualAdjustments = 0
                };
            }

            var firstEntry = ratingHistory.First();
            var lastEntry = ratingHistory.Last();
            var matchEntries = ratingHistory.Where(rh => rh.MatchId.HasValue).ToList();
            var manualEntries = ratingHistory.Where(rh => rh.ChangeReason == "Manual").ToList();

            var changeReasonBreakdown = ratingHistory
                .GroupBy(rh => rh.ChangeReason)
                .ToDictionary(g => g.Key, g => g.Count());

            return new RatingStatisticsDto
            {
                UserId = userId,
                UserName = $"{user.FirstName} {user.LastName}",
                CurrentRating = user.Rating,
                StartingRating = firstEntry.NewRating, // First recorded rating
                HighestRating = ratingHistory.Max(rh => rh.NewRating),
                LowestRating = ratingHistory.Min(rh => rh.NewRating),
                TotalRatingChanges = ratingHistory.Count,
                MatchesPlayed = matchEntries.Count,
                ManualAdjustments = manualEntries.Count,
                FirstRatingChange = firstEntry.CreatedAt,
                LastRatingChange = lastEntry.CreatedAt,
                ChangeReasonBreakdown = changeReasonBreakdown
            };
        }

        public async Task<IEnumerable<RatingHistory>> GetRecentRatingChangesAsync(int userId, int count = 10)
        {
            return await _context.RatingHistories
                .Include(rh => rh.User)
                .Include(rh => rh.Match)
                .Where(rh => rh.UserId == userId)
                .OrderByDescending(rh => rh.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<float?> GetUserRatingAtDateAsync(int userId, DateTime date)
        {
            var lastRatingChange = await _context.RatingHistories
                .Where(rh => rh.UserId == userId && rh.CreatedAt <= date)
                .OrderByDescending(rh => rh.CreatedAt)
                .FirstOrDefaultAsync();

            return lastRatingChange?.NewRating;
        }

        public async Task<IEnumerable<RatingHistory>> GetRatingChangesByReasonAsync(int userId, string changeReason)
        {
            return await _context.RatingHistories
                .Include(rh => rh.User)
                .Include(rh => rh.Match)
                .Where(rh => rh.UserId == userId && rh.ChangeReason == changeReason)
                .OrderByDescending(rh => rh.CreatedAt)
                .ToListAsync();
        }
    }
}