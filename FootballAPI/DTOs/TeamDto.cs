namespace FootballAPI.DTOs
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PlayerDto> CurrentPlayers { get; set; }
    }

    public class CreateTeamDto
    {
        public string Name { get; set; }
    }

    public class UpdateTeamDto
    {
        public string Name { get; set; }
    }
}