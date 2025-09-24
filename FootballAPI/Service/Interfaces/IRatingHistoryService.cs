using FootballAPI.DTOs;

namespace FootballAPI.Service.Interfaces
{
    public interface IRatingHistoryService
    {
        Task<IEnumerable<RatingHistoryDto>> GetUserRatingHistoryAsync(int userId, GetRatingHistoryDto? filters = null);
        Task<IEnumerable<RatingHistoryDto>> GetMatchRatingChangesAsync(int matchId);
        Task<RatingTrendDto> GetUserRatingTrendAsync(int userId, int? lastNMatches = null);
        Task<RatingStatisticsDto> GetUserRatingStatisticsAsync(int userId);
        Task<RatingHistoryDto> CreateRatingHistoryAsync(CreateRatingHistoryDto dto);
        Task<IEnumerable<RatingHistoryDto>> GetRecentRatingChangesAsync(int userId, int count = 10);
        Task<float?> GetUserRatingAtDateAsync(int userId, DateTime date);
        Task<IEnumerable<RatingHistoryDto>> GetRatingChangesByReasonAsync(int userId, string changeReason);
        Task<bool> DeleteRatingHistoryAsync(int id);
    }
}