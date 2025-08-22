namespace FootballAPI.DTOs
{
    public class CreatePlayerMatchStatsDto
    {
        public int PlayerId { get; set; }
        public int Goals { get; set; } = 0;
        public int Assists { get; set; } = 0;
        public int TeamNumber { get; set; }
        public float? Rating { get; set; }
    }

    public class UpdatePlayerMatchStatsDto
    {
        public int Goals { get; set; }
        public int Assists { get; set; }
        public float? Rating { get; set; }
    }

    public class PlayerMatchStatsDto
    {
        public int Id { get; set; }
        public int MatchHistoryId { get; set; }
        public int PlayerId { get; set; }
        public string PlayerFirstName { get; set; } = string.Empty;
        public string PlayerLastName { get; set; } = string.Empty;
        public string PlayerUsername { get; set; } = string.Empty;
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int TeamNumber { get; set; }
        public float? Rating { get; set; }
        public string MatchTitle { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
    }

    public class PlayerStatsAggregateDto
    {
        public int PlayerId { get; set; }
        public string PlayerFirstName { get; set; } = string.Empty;
        public string PlayerLastName { get; set; } = string.Empty;
        public string PlayerUsername { get; set; } = string.Empty;
        public int TotalMatches { get; set; }
        public int TotalGoals { get; set; }
        public int TotalAssists { get; set; }
        public float AverageRating { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
    }
}