using System.ComponentModel.DataAnnotations;

namespace FootballAPI.DTOs
{
    public class UpdatePlayerMatchHistoryDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PlayerId must be greater than 0")]
        public int PlayerId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TeamId must be greater than 0")]
        public int TeamId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "MatchId must be greater than 0")]
        public int MatchId { get; set; }

        [Range(0.0f, 10000.0f, ErrorMessage = "PerformanceRating must be between 0.0 and 10000.0")]
        public float PerformanceRating { get; set; }
    }
}