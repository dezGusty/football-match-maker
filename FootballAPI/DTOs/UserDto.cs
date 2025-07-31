using FootballAPI.Models;

namespace FootballAPI.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public UserRole Role { get; set; }
        // Password is not included in response DTOs for security
    }
}