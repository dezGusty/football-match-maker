namespace FootballAPI.DTOs
{
    public class BatchUpdatePlayersDto
    {
        public int MatchId { get; set; }
        public int TeamAId { get; set; }
        public int TeamBId { get; set; }
        public List<PlayerTeamDto> Additions { get; set; } = new List<PlayerTeamDto>();
        public List<PlayerTeamDto> Removals { get; set; } = new List<PlayerTeamDto>();
    }

    public class PlayerTeamDto
    {
        public int PlayerId { get; set; }
        public int TeamId { get; set; }
    }
}