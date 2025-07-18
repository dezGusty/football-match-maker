using System.ComponentModel.DataAnnotations;

namespace FootballAPI.DTOs
{
    public class PlayerMatchHistoryDto
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public PlayerDto? Player { get; set; }
        public int TeamId { get; set; }
        public TeamDto? Team { get; set; }
        public int MatchId { get; set; }
        public MatchDto? Match { get; set; }
        public float PerformanceRating { get; set; }
        public DateTime RecordDate { get; set; }
    }
}