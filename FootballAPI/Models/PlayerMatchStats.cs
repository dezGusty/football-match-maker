using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballAPI.Models
{
    public class PlayerMatchStats
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MatchHistoryId { get; set; }

        [ForeignKey("MatchHistoryId")]
        public virtual MatchHistory MatchHistory { get; set; }

        [Required]
        public int PlayerId { get; set; }

        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }

        [Required]
        [Range(0, 20, ErrorMessage = "Goals must be between 0 and 20")]
        public int Goals { get; set; } = 0;

        [Required]
        [Range(0, 20, ErrorMessage = "Assists must be between 0 and 20")]
        public int Assists { get; set; } = 0;

        [Required]
        [Range(1, 2, ErrorMessage = "Team number must be 1 or 2")]
        public int TeamNumber { get; set; }

        [Range(0.0f, 10.0f, ErrorMessage = "Rating must be between 0.0 and 10.0")]
        public float? Rating { get; set; }
    }
}