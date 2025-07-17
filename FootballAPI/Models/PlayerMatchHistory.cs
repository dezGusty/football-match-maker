using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballAPI.Models
{
    public class PlayerMatchHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PlayerId { get; set; }
        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }

        [Required]
        public int TeamId { get; set; }
        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }

        [Required]
        public int MatchId { get; set; }
        [ForeignKey("MatchId")]
        public virtual Match Match { get; set; }

        public float PerformanceRating { get; set; } = 0.0f;

        public DateTime RecordDate { get; set; } = DateTime.Now;
    }
}
