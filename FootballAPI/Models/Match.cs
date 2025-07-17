using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballAPI.Models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime MatchDate { get; set; }

        [Required]
        public int TeamAId { get; set; }
        [ForeignKey("TeamAId")]
        public virtual Team TeamA { get; set; }

        [Required]
        public int TeamBId { get; set; }
        [ForeignKey("TeamBId")]
        public virtual Team TeamB { get; set; }

        public int TeamAGoals { get; set; } = 0;
        public int TeamBGoals { get; set; } = 0;

        public virtual ICollection<PlayerMatchHistory> PlayerHistory { get; set; } = new List<PlayerMatchHistory>();
    }
}
