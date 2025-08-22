using FootballAPI.Models;

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
    }

    public class CreateMatchDto
    {
        public DateTime MatchDate { get; set; }
        public Status Status { get; set; } = Status.Open;
        public string? Location { get; set; }
        public decimal? Cost { get; set; }
        public int OrganiserId { get; set; }
    }

    public class UpdateMatchDto
    {
        public DateTime MatchDate { get; set; }
        public bool IsPublic { get; set; }
        public Status Status { get; set; }
        public string? Location { get; set; }
        public decimal? Cost { get; set; }
    }
}