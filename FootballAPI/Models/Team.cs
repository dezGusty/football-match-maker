using System.ComponentModel.DataAnnotations;

namespace FootballAPI.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public virtual ICollection<PlayerMatchHistory> PlayerHistory { get; set; } = new List<PlayerMatchHistory>();

        public virtual ICollection<Match> HomeMatches { get; set; } = new List<Match>();
        public virtual ICollection<Match> AwayMatches { get; set; } = new List<Match>();
    }
}