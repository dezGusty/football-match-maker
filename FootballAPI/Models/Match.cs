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


        public virtual ICollection<PlayerMatchHistory> PlayerHistory { get; set; } = new List<PlayerMatchHistory>();
    }
}
