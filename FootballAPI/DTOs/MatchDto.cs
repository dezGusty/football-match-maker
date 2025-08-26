using FootballAPI.Models.Enums;

namespace FootballAPI.DTOs
{
    public class MatchDto
    {
        public int Id { get; set; }
        public DateTime MatchDate { get; set; }
        public bool IsPublic { get; set; }
        public Status Status { get; set; }
        public string? Location { get; set; }
        public decimal? Cost { get; set; }
        public int OrganiserId { get; set; }
        public string? TeamAName { get; set; }
        public string? TeamBName { get; set; }
        public int? TeamAId { get; set; }
        public int? TeamBId { get; set; }
        public int? ScoreA { get; set; }
        public int? ScoreB { get; set; }
        public List<PlayerHistoryDto> PlayerHistory { get; set; } = new List<PlayerHistoryDto>();
    }
    
    public class PlayerHistoryDto
    {
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public PlayerStatus Status { get; set; }
        public UserDto User { get; set; } = null!;
    }

    public class CreateMatchDto
    {
        public string MatchDate { get; set; }
        public Status Status { get; set; } = Status.Open;
        public string? Location { get; set; }
        public decimal? Cost { get; set; }
        public string? TeamAName { get; set; }
        public string? TeamBName { get; set; }
    }

    public class UpdateMatchDto
    {
        public string MatchDate { get; set; }
        public bool IsPublic { get; set; }
        public Status Status { get; set; }
        public string? Location { get; set; }
        public decimal? Cost { get; set; }
    }

    public class AddPlayerToMatchDto
    {
        public int UserId { get; set; }
        public int TeamId { get; set; }
    }

    public class MovePlayerDto
    {
        public int NewTeamId { get; set; }
    }

    public class MatchDetailsDto
    {
        public int Id { get; set; }
        public DateTime MatchDate { get; set; }
        public bool IsPublic { get; set; }
        public Status Status { get; set; }
        public string? Location { get; set; }
        public decimal? Cost { get; set; }
        public int OrganiserId { get; set; }
        public List<TeamWithPlayersDto> Teams { get; set; } = new List<TeamWithPlayersDto>();
        public int TotalPlayers { get; set; }
    }

    public class TeamWithPlayersDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public int MatchTeamId { get; set; }
        public List<PlayerInMatchDto> Players { get; set; } = new List<PlayerInMatchDto>();
        public int PlayerCount => Players.Count;
    }

    public class PlayerInMatchDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public int Speed { get; set; }
        public int Stamina { get; set; }
        public int Errors { get; set; }
        public PlayerStatus Status { get; set; }
    }
}