using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballAPI.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Range(0.0f, 10000.0f, ErrorMessage = "Rating must be between 0.0 and 10000.0")]
        public float Rating { get; set; } = 0.0f;

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        public bool IsAvailable { get; set; } = false;
        public bool IsEnabled { get; set; } = true;

        [Range(1, 3, ErrorMessage = "Speed must be between 1 (Low) and 3 (High)")]
        public int Speed { get; set; } = 2; // 1 = Low, 2 = Medium, 3 = High

        [Range(1, 3, ErrorMessage = "Stamina must be between 1 (Low) and 3 (High)")]
        public int Stamina { get; set; } = 2; // 1 = Low, 2 = Medium, 3 = High

        [Range(1, 3, ErrorMessage = "Errors must be between 1 (Low) and 3 (High)")]
        public int Errors { get; set; } = 2; // 1 = Low, 2 = Medium, 3 = High (Low = Few errors, High = Many errors)

        [StringLength(500)]
        public string? ProfileImagePath { get; set; }

        public virtual ICollection<PlayerMatchHistory> MatchHistory { get; set; } = new List<PlayerMatchHistory>();
    }
}