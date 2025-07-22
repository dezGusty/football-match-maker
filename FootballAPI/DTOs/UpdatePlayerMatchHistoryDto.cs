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

        [Range(0.0f, 10.0f, ErrorMessage = "PerformanceRating trebuie să fie între 0.0 și 10.0")]
        public float PerformanceRating { get; set; }
    }
}