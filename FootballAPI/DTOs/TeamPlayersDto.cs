using System.ComponentModel.DataAnnotations;
using FootballAPI.Models;

namespace FootballAPI.DTOs
{
    public class TeamPlayersDto
    {
        public int Id { get; set; }
        public int MatchTeamId { get; set; }
        public int PlayerId { get; set; }
        public PlayerStatus Status { get; set; }
        public MatchTeamsDto MatchTeam { get; set; }
        public PlayerDto Player { get; set; }
    }

    public class CreateTeamPlayersDto
    {
        [Required]
        public int MatchTeamId { get; set; }
        
        [Required]
        public int PlayerId { get; set; }
        
        public PlayerStatus Status { get; set; } = PlayerStatus.Open;
    }

    public class UpdateTeamPlayersDto
    {
        [Required]
        public int MatchTeamId { get; set; }
        
        [Required]
        public int PlayerId { get; set; }
        
        public PlayerStatus Status { get; set; }
    }
}