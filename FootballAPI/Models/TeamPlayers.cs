using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FootballAPI.Models.Enums;

namespace FootballAPI.Models
{
  public class TeamPlayers
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public int MatchTeamId { get; set; }

    [Required]
    public int UserId { get; set; }

    public PlayerStatus Status { get; set; }

    [ForeignKey("MatchTeamId")]
    public virtual MatchTeams MatchTeam { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
  }
}

