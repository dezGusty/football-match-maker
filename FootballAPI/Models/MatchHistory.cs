using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballAPI.Models
{
    public class MatchHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MatchId { get; set; }

        [ForeignKey("MatchId")]
        public virtual Match Match { get; set; }

        [Required]
        [Range(0, 50, ErrorMessage = "Team 1 score must be between 0 and 50")]
        public int Team1Score { get; set; } = 0;

        [Required]
        [Range(0, 50, ErrorMessage = "Team 2 score must be between 0 and 50")]
        public int Team2Score { get; set; } = 0;

        [Required]
        [Range(1, 300, ErrorMessage = "Duration must be between 1 and 300 minutes")]
        public int Duration { get; set; }

        [Required]
        public DateTime CompletedAt { get; set; } = DateTime.Now;

        public virtual ICollection<PlayerMatchStats> PlayerStats { get; set; } = new List<PlayerMatchStats>();
    }
}