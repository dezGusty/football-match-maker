using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballAPI.Models
{
    public class RatingHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public float NewRating { get; set; }

        [Required]
        [StringLength(50)]
        public string ChangeReason { get; set; } = string.Empty; // "Match", "Manual", "Import"

        public int? MatchId { get; set; }

        [StringLength(50)]
        public string? RatingSystem { get; set; } // "Performance", "Linear", etc.

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("MatchId")]
        public virtual Match? Match { get; set; }
    }
}