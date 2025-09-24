using FootballAPI.Models;
using FootballAPI.DTOs;

namespace FootballAPI.Repository.Interfaces
{
    public interface IRatingHistoryRepository
    {
        Task<IEnumerable<RatingHistory>> GetAllAsync();
        Task<RatingHistory?> GetByIdAsync(int id);
        Task<IEnumerable<RatingHistory>> GetByUserIdAsync(int userId, GetRatingHistoryDto? filters = null);
        Task<IEnumerable<RatingHistory>> GetByMatchIdAsync(int matchId);
        Task<RatingHistory> CreateAsync(RatingHistory ratingHistory);
        Task<RatingHistory> UpdateAsync(RatingHistory ratingHistory);
        Task<bool> DeleteAsync(int id);
        Task<RatingStatisticsDto> GetUserRatingStatisticsAsync(int userId);
        Task<IEnumerable<RatingHistory>> GetRecentRatingChangesAsync(int userId, int count = 10);
        Task<float?> GetUserRatingAtDateAsync(int userId, DateTime date);
        Task<IEnumerable<RatingHistory>> GetRatingChangesByReasonAsync(int userId, string changeReason);
    }
}