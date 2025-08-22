namespace FootballAPI.DTOs
{
    public class JoinMatchDto
    {
        public int MatchId { get; set; }
    }

    public class UpdatePlayerTeamDto
    {
        public int MatchId { get; set; }
        public int PlayerId { get; set; }
        public int TeamNumber { get; set; }
    }

    public class MatchPlayerDto
    {
        public int MatchId { get; set; }
        public string MatchTitle { get; set; } = string.Empty;
        public int PlayerId { get; set; }
        public string PlayerFirstName { get; set; } = string.Empty;
        public string PlayerLastName { get; set; } = string.Empty;
        public string PlayerUsername { get; set; } = string.Empty;
        public float PlayerRating { get; set; }
        public int TeamNumber { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsConfirmed { get; set; }
    }

    public class TeamDto
    {
        public int TeamNumber { get; set; }
        public List<MatchPlayerDto> Players { get; set; } = new List<MatchPlayerDto>();
        public int PlayerCount { get; set; }
        public float AverageRating { get; set; }
    }
}