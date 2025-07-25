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
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Range(0.0f, 10.0f, ErrorMessage = "Rating must be between 0.0 and 10.0")]
        public float Rating { get; set; } = 0.0f;

        public bool IsAvailable { get; set; } = false;
        public bool IsEnabled { get; set; } = true;

        public int? CurrentTeamId { get; set; }

        [ForeignKey("CurrentTeamId")]
        public virtual Team CurrentTeam { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public virtual ICollection<PlayerMatchHistory> MatchHistory { get; set; } = new List<PlayerMatchHistory>();
    }
}
