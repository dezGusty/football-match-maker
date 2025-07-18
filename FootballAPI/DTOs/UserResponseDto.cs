namespace FootballAPI.DTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Token { get; set; } // For JWT token if using authentication
    }
}