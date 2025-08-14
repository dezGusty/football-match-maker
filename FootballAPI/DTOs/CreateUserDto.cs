using System.ComponentModel.DataAnnotations;
using FootballAPI.Models;

namespace FootballAPI.DTOs
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; }
    }
}