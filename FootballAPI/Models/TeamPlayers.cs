using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FootballAPI.Models
{
  public class TeamPlayers
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public int MatchTeamId { get; set; }

    [Required]
    public int PlayerId { get; set; }

    public PlayerStatus Status { get; set; }

    [ForeignKey("MatchTeamId")]
    public virtual MatchTeams MatchTeam { get; set; } = null!;

    [ForeignKey("PlayerId")]
    public virtual Player Player { get; set; } = null!;
  }
  public enum PlayerStatus
  {
    addedByOrganiser = 1,
    joined = 2,
    Open = 3
  }
}

