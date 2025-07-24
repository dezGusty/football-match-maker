using System.ComponentModel.DataAnnotations;

namespace FootballAPI.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
        public string Username { get; set; }

        [StringLength(50, ErrorMessage = "Role cannot exceed 50 characters")]
        public string Role { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }
    }
}