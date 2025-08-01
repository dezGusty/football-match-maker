using System.ComponentModel.DataAnnotations;
using FootballAPI.Models;

namespace FootballAPI.DTOs
{
    public class CreateUserDto
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public string? ImageUrl { get; set; }

        // Pentru PLAYER
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public float? Rating { get; set; }
    }
}