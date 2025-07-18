namespace FootballAPI.DTOs
{
    public class PlayerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public float Rating { get; set; }
        public bool IsAvailable { get; set; }
        public int? CurrentTeamId { get; set; }
       
    }

    public class CreatePlayerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public float Rating { get; set; }
        public bool IsAvailable { get; set; }
        public int? CurrentTeamId { get; set; }
    }

    public class UpdatePlayerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public float Rating { get; set; }
        public bool IsAvailable { get; set; }
        public int? CurrentTeamId { get; set; }
    }
}