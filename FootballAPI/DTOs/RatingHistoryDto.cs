using System.ComponentModel.DataAnnotations;
using FootballAPI.Models.Enums;

namespace FootballAPI.DTOs
{
    public class RatingHistoryDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public float NewRating { get; set; }
        public string ChangeReason { get; set; } = string.Empty;
        public int? MatchId { get; set; }
        public string? MatchDetails { get; set; } // "vs Team A (2-1)" 
        public string? RatingSystem { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RatingTrendDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public float CurrentRating { get; set; }
        public float HighestRating { get; set; }
        public float LowestRating { get; set; }
        public float AverageRating { get; set; }
        public int TotalMatches { get; set; }
        public DateTime? LastMatchDate { get; set; }
        public List<RatingPointDto> RatingPoints { get; set; } = new List<RatingPointDto>();
    }

    public class RatingPointDto
    {
        public DateTime Date { get; set; }
        public float Rating { get; set; }
        public string ChangeReason { get; set; } = string.Empty;
        public string? MatchDetails { get; set; }
    }

    public class GetRatingHistoryDto
    {
        public int? MatchId { get; set; }
        public string? ChangeReason { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class CreateRatingHistoryDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "UserId must be greater than 0")]
        public int UserId { get; set; }

        [Required]
        public float NewRating { get; set; }

        [Required]
        [StringLength(50)]
        public string ChangeReason { get; set; } = string.Empty;

        public int? MatchId { get; set; }

        [StringLength(50)]
        public string? RatingSystem { get; set; }
    }

    public class RatingStatisticsDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public float CurrentRating { get; set; }
        public float StartingRating { get; set; }
        public float HighestRating { get; set; }
        public float LowestRating { get; set; }
        public int TotalRatingChanges { get; set; }
        public int MatchesPlayed { get; set; }
        public int ManualAdjustments { get; set; }
        public DateTime? FirstRatingChange { get; set; }
        public DateTime? LastRatingChange { get; set; }
        public Dictionary<string, int> ChangeReasonBreakdown { get; set; } = new Dictionary<string, int>();
    }
}