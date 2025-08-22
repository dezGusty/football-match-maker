using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballAPI.Models
{
    public class MatchPlayer
    {
        [Required]
        public int MatchId { get; set; }

        [ForeignKey("MatchId")]
        public virtual Match Match { get; set; }

        [Required]
        public int PlayerId { get; set; }

        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "Team number must be 1 or 2")]
        public int TeamNumber { get; set; }

        [Required]
        public DateTime JoinedAt { get; set; } = DateTime.Now;

        [Required]
        public bool IsConfirmed { get; set; } = true;
    }
}