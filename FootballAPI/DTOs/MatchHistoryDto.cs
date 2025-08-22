namespace FootballAPI.DTOs
{
    public class CreateMatchHistoryDto
    {
        public int MatchId { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public int Duration { get; set; }
        public List<CreatePlayerMatchStatsDto> PlayerStats { get; set; } = new List<CreatePlayerMatchStatsDto>();
    }

    public class UpdateMatchHistoryDto
    {
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public int Duration { get; set; }
    }

    public class MatchHistoryDto
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public string MatchTitle { get; set; } = string.Empty;
        public string OrganiserUsername { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime MatchDateTime { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public int Duration { get; set; }
        public DateTime CompletedAt { get; set; }
        public string Result { get; set; } = string.Empty;
        public List<PlayerMatchStatsDto> PlayerStats { get; set; } = new List<PlayerMatchStatsDto>();
    }

    public class MatchHistorySummaryDto
    {
        public int Id { get; set; }
        public string MatchTitle { get; set; } = string.Empty;
        public string OrganiserUsername { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime MatchDateTime { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public string Result { get; set; } = string.Empty;
        public DateTime CompletedAt { get; set; }
    }
}