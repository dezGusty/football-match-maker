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
        
        // Navigation properties
        public virtual ICollection<MatchTeams> MatchTeams { get; set; } = new List<MatchTeams>();
    }
}