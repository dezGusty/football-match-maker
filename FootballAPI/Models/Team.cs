using System.ComponentModel.DataAnnotations;

namespace FootballAPI.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Player> CurrentPlayers { get; set; } = new List<Player>();
        public virtual ICollection<PlayerMatchHistory> PlayerHistory { get; set; } = new List<PlayerMatchHistory>();

        // Navigații pentru Match-uri (Match.TeamA / TeamB)
        public virtual ICollection<Match> HomeMatches { get; set; } = new List<Match>();
        public virtual ICollection<Match> AwayMatches { get; set; } = new List<Match>();
    }
}
