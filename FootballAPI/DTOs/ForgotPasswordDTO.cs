using System.ComponentModel.DataAnnotations;
using FootballAPI.Models;

namespace FootballAPI.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
    }
}