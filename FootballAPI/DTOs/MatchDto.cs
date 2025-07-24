namespace FootballAPI.DTOs
{
    public class MatchDto
    {
        public int Id { get; set; }
        public DateTime MatchDate { get; set; }
        public int TeamAId { get; set; }
        public TeamDto TeamA { get; set; }
        public int TeamBId { get; set; }
        public TeamDto TeamB { get; set; }
        public int TeamAGoals { get; set; }
        public int TeamBGoals { get; set; }
        public List<PlayerMatchHistoryDto> PlayerHistory { get; set; }
    }

    public class CreateMatchDto
    {
        public DateTime MatchDate { get; set; }
        public int TeamAId { get; set; }
        public int TeamBId { get; set; }
        public int TeamAGoals { get; set; }
        public int TeamBGoals { get; set; }
    }

    public class UpdateMatchDto
    {
        public DateTime MatchDate { get; set; }
        public int TeamAId { get; set; }
        public int TeamBId { get; set; }
        public int TeamAGoals { get; set; }
        public int TeamBGoals { get; set; }
    }


}