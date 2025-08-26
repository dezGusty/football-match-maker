using System.ComponentModel.DataAnnotations;
using FootballAPI.Models.Enums;

namespace FootballAPI.DTOs
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class CreateTeamDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Team name must be between 2 and 100 characters")]
        public string Name { get; set; } = null!;
    }

    public class UpdateTeamDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Team name must be between 2 and 100 characters")]
        public string Name { get; set; } = null!;
    }

    public class MatchTeamsDto
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public int Goals { get; set; }
        public MatchDto Match { get; set; } = null!;
        public TeamDto Team { get; set; } = null!;
    }

    public class CreateMatchTeamsDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "MatchId must be greater than 0")]
        public int MatchId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TeamId must be greater than 0")]
        public int TeamId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Goals cannot be negative")]
        public int Goals { get; set; } = 0;
    }

    public class UpdateMatchTeamsDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "MatchId must be greater than 0")]
        public int MatchId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TeamId must be greater than 0")]
        public int TeamId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Goals cannot be negative")]
        public int Goals { get; set; }
    }

    public class TeamPlayersDto
    {
        public int Id { get; set; }
        public int MatchTeamId { get; set; }
        public int UserId { get; set; }
        public PlayerStatus Status { get; set; }
        public MatchTeamsDto MatchTeam { get; set; } = null!;
        public UserDto User { get; set; } = null!;
    }

    public class CreateTeamPlayersDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "MatchTeamId must be greater than 0")]
        public int MatchTeamId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "UserId must be greater than 0")]
        public int UserId { get; set; }
        
        public PlayerStatus Status { get; set; } = PlayerStatus.Open;
    }

    public class UpdateTeamPlayersDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "MatchTeamId must be greater than 0")]
        public int MatchTeamId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "UserId must be greater than 0")]
        public int UserId { get; set; }
        
        public PlayerStatus Status { get; set; }
    }
}