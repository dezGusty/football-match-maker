using FootballAPI.Models;
namespace FootballAPI.DTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
    }
}