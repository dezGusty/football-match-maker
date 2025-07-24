namespace FootballAPI.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        // Password is not included in response DTOs for security
    }
}