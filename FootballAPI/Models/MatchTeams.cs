using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballAPI.Models
{
  public class MatchTeams
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public int MatchId { get; set; }

    [Required]
    public int TeamId { get; set; }

    public int Goals { get; set; } = 0;

    [ForeignKey("MatchId")]
    public virtual Match Match { get; set; } = null!;

    [ForeignKey("TeamId")]
    public virtual Team Team { get; set; } = null!;

    // Navigation properties
    public virtual ICollection<TeamPlayers> TeamPlayers { get; set; } = new List<TeamPlayers>();

  }
}
