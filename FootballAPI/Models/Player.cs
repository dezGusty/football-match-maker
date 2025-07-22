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

        public float Rating { get; set; } = 0.0f;

        public bool IsAvailable { get; set; } = false;
        public bool IsEnabled { get; set; } = true;

        public int? CurrentTeamId { get; set; }

        [ForeignKey("CurrentTeamId")]
        public virtual Team CurrentTeam { get; set; }

        public virtual ICollection<PlayerMatchHistory> MatchHistory { get; set; } = new List<PlayerMatchHistory>();
    }
}
