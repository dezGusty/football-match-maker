using System.ComponentModel.DataAnnotations;

namespace FootballAPI.DTOs
{
    public class MatchTeamsDto
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public int Goals { get; set; }
        public MatchDto Match { get; set; }
        public TeamDto Team { get; set; }
    }

    public class CreateMatchTeamsDto
    {
        [Required]
        public int MatchId { get; set; }

        [Required]
        public int TeamId { get; set; }

        public int Goals { get; set; } = 0;
    }

    public class UpdateMatchTeamsDto
    {
        [Required]
        public int MatchId { get; set; }

        [Required]
        public int TeamId { get; set; }

        public int Goals { get; set; }
    }
}