using FootballAPI.Models;
namespace FootballAPI.DTOs
{
    public class UserWithImageDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public string ImageUrl { get; set; }
    }
}