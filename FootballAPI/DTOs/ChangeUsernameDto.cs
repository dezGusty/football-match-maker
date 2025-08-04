using System.ComponentModel.DataAnnotations;

namespace FootballAPI.DTOs
{
    public class ChangeUsernameDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
        public string NewUsername { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}