using FootballAPI.Models;

namespace FootballAPI.DTOs
{
    public class MatchDto
    {
        public int Id { get; set; }
        public DateTime MatchDate { get; set; }
        public bool IsPublic { get; set; }
        public Status Status { get; set; }
        public List<PlayerMatchHistoryDto> PlayerHistory { get; set; } = new List<PlayerMatchHistoryDto>();
    }

    public class CreateMatchDto
    {
        public DateTime MatchDate { get; set; }
        public bool IsPublic { get; set; } = false;
        public Status Status { get; set; } = Status.Open;
    }

    public class UpdateMatchDto
    {
        public DateTime MatchDate { get; set; }
        public bool IsPublic { get; set; }
        public Status Status { get; set; }
    }
}