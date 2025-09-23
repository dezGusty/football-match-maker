using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository.Interfaces;
using FootballAPI.Service.Interfaces;

namespace FootballAPI.Service
{
    public class RatingHistoryService : IRatingHistoryService
    {
        private readonly IRatingHistoryRepository _ratingHistoryRepository;

        public RatingHistoryService(IRatingHistoryRepository ratingHistoryRepository)
        {
            _ratingHistoryRepository = ratingHistoryRepository;
        }

        public async Task<IEnumerable<RatingHistoryDto>> GetUserRatingHistoryAsync(int userId, GetRatingHistoryDto? filters = null)
        {
            var ratingHistory = await _ratingHistoryRepository.GetByUserIdAsync(userId, filters);
            return ratingHistory.Select(MapToDto);
        }

        public async Task<IEnumerable<RatingHistoryDto>> GetMatchRatingChangesAsync(int matchId)
        {
            var ratingHistory = await _ratingHistoryRepository.GetByMatchIdAsync(matchId);
            return ratingHistory.Select(MapToDto);
        }

        public async Task<RatingTrendDto> GetUserRatingTrendAsync(int userId, int? lastNMatches = null)
        {
            var filters = new GetRatingHistoryDto();
            if (lastNMatches.HasValue)
            {
                filters.PageSize = lastNMatches.Value;
            }

            var ratingHistory = await _ratingHistoryRepository.GetByUserIdAsync(userId, filters);
            var historyList = ratingHistory.ToList();

            if (!historyList.Any())
            {
                // Return empty trend if no history
                return new RatingTrendDto
                {
                    UserId = userId,
                    UserName = "Unknown User",
                    RatingPoints = new List<RatingPointDto>()
                };
            }

            var firstUser = historyList.First().User;
            var ratingPoints = historyList.Select(rh => new RatingPointDto
            {
                Date = rh.CreatedAt,
                Rating = rh.NewRating,
                ChangeReason = rh.ChangeReason,
                MatchDetails = GetMatchDetails(rh)
            }).ToList();

            var matchRatings = historyList.Where(rh => rh.MatchId.HasValue).ToList();

            return new RatingTrendDto
            {
                UserId = userId,
                UserName = $"{firstUser.FirstName} {firstUser.LastName}",
                CurrentRating = historyList.Last().NewRating,
                HighestRating = historyList.Max(rh => rh.NewRating),
                LowestRating = historyList.Min(rh => rh.NewRating),
                AverageRating = historyList.Average(rh => rh.NewRating),
                TotalMatches = matchRatings.Count,
                LastMatchDate = matchRatings.Any() ? matchRatings.Max(rh => rh.CreatedAt) : null,
                RatingPoints = ratingPoints
            };
        }

        public async Task<RatingStatisticsDto> GetUserRatingStatisticsAsync(int userId)
        {
            return await _ratingHistoryRepository.GetUserRatingStatisticsAsync(userId);
        }

        public async Task<RatingHistoryDto> CreateRatingHistoryAsync(CreateRatingHistoryDto dto)
        {
            var ratingHistory = new RatingHistory
            {
                UserId = dto.UserId,
                NewRating = dto.NewRating,
                ChangeReason = dto.ChangeReason,
                MatchId = dto.MatchId,
                RatingSystem = dto.RatingSystem,
                CreatedAt = DateTime.UtcNow
            };

            var createdHistory = await _ratingHistoryRepository.CreateAsync(ratingHistory);
            return MapToDto(createdHistory);
        }

        public async Task<IEnumerable<RatingHistoryDto>> GetRecentRatingChangesAsync(int userId, int count = 10)
        {
            var ratingHistory = await _ratingHistoryRepository.GetRecentRatingChangesAsync(userId, count);
            return ratingHistory.Select(MapToDto);
        }

        public async Task<float?> GetUserRatingAtDateAsync(int userId, DateTime date)
        {
            return await _ratingHistoryRepository.GetUserRatingAtDateAsync(userId, date);
        }

        public async Task<IEnumerable<RatingHistoryDto>> GetRatingChangesByReasonAsync(int userId, string changeReason)
        {
            var ratingHistory = await _ratingHistoryRepository.GetRatingChangesByReasonAsync(userId, changeReason);
            return ratingHistory.Select(MapToDto);
        }

        public async Task<bool> DeleteRatingHistoryAsync(int id)
        {
            return await _ratingHistoryRepository.DeleteAsync(id);
        }

        private RatingHistoryDto MapToDto(RatingHistory ratingHistory)
        {
            return new RatingHistoryDto
            {
                Id = ratingHistory.Id,
                UserId = ratingHistory.UserId,
                UserName = $"{ratingHistory.User.FirstName} {ratingHistory.User.LastName}",
                NewRating = ratingHistory.NewRating,
                ChangeReason = ratingHistory.ChangeReason,
                MatchId = ratingHistory.MatchId,
                MatchDetails = GetMatchDetails(ratingHistory),
                RatingSystem = ratingHistory.RatingSystem,
                CreatedAt = ratingHistory.CreatedAt
            };
        }

        private string? GetMatchDetails(RatingHistory ratingHistory)
        {
            if (ratingHistory.Match == null)
                return null;

            var match = ratingHistory.Match;
            var location = !string.IsNullOrEmpty(match.Location) ? $" at {match.Location}" : "";
            return $"Match on {match.MatchDate:dd/MM/yyyy}{location}";
        }
    }
}