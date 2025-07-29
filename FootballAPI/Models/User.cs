using System.ComponentModel.DataAnnotations;

namespace FootballAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [StringLength(50)]
        public string Role { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }
    }
}
