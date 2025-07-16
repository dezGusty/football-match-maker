namespace FootballAPI.Models
{
    public class Player
    {
        public required int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public float Rating { get; set; } = 0.0f;
        public bool IsAvailable { get; set; } = false;
    }
}
