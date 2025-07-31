using System.ComponentModel.DataAnnotations;
using FootballAPI.Models;

namespace FootballAPI.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Email is not valid")]
        public string Email { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }
    }
}