namespace FootballAPI.DTOs
{
    public class CreateMatchDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public bool IsPublic { get; set; } = false;
        public int MaxPlayers { get; set; } = 12;
    }

    public class UpdateMatchDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public bool IsPublic { get; set; }
        public int MaxPlayers { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class MatchDto
    {
        public int Id { get; set; }
        public int OrganiserId { get; set; }
        public string OrganiserUsername { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public bool IsPublic { get; set; }
        public string Status { get; set; } = string.Empty;
        public int MaxPlayers { get; set; }
        public int CurrentPlayerCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<MatchPlayerDto> Players { get; set; } = new List<MatchPlayerDto>();
        public MatchHistoryDto? MatchHistory { get; set; }
    }

    public class PublicMatchDto
    {
        public int Id { get; set; }
        public string OrganiserUsername { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public int MaxPlayers { get; set; }
        public int CurrentPlayerCount { get; set; }
        public int AvailableSpots { get; set; }
    }
}