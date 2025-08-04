using System.ComponentModel.DataAnnotations;

namespace FootballAPI.DTOs
{
    public class ForgottenPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
    }
}