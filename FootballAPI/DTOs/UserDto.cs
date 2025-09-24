using System.ComponentModel.DataAnnotations;
using FootballAPI.Models.Enums;

namespace FootballAPI.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public UserRole Role { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        [Range(0.0f, 10.0f, ErrorMessage = "Rating must be between 0.0 and 10.0")]
        public float Rating { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        [Range(1, 4, ErrorMessage = "Speed must be between 1 (Low) and 4 (Extreme)")]
        public int Speed { get; set; }

        [Range(1, 4, ErrorMessage = "Stamina must be between 1 (Low) and 4 (Extreme)")]
        public int Stamina { get; set; }

        [Range(1, 4, ErrorMessage = "Errors must be between 1 (Low) and 4 (Extreme)")]
        public int Errors { get; set; }

        public string? ProfileImageUrl { get; set; }
    }

    public class CreateUserDto
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
        public string Username { get; set; } = null!;

        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string? Password { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [Range(0.0f, 10.0f, ErrorMessage = "Rating must be between 0.0 and 10.0")]
        public float Rating { get; set; } = 0.0f;

        [Range(1, 4, ErrorMessage = "Speed must be between 1 (Low) and 4 (Extreme)")]
        public int Speed { get; set; } = 2;

        [Range(1, 4, ErrorMessage = "Stamina must be between 1 (Low) and 4 (Extreme)")]
        public int Stamina { get; set; } = 2;

        [Range(1, 4, ErrorMessage = "Errors must be between 1 (Low) and 4 (Extreme)")]
        public int Errors { get; set; } = 2;
    }

    public class UpdateUserDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
        public string Username { get; set; } = null!;

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [Range(0.0f, 10.0f, ErrorMessage = "Rating must be between 0.0 and 10.0")]
        public float Rating { get; set; }

        [Range(1, 4, ErrorMessage = "Speed must be between 1 (Low) and 4 (Extreme)")]
        public int Speed { get; set; }

        [Range(1, 4, ErrorMessage = "Stamina must be between 1 (Low) and 4 (Extreme)")]
        public int Stamina { get; set; }

        [Range(1, 4, ErrorMessage = "Errors must be between 1 (Low) and 4 (Extreme)")]
        public int Errors { get; set; }
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public UserRole Role { get; set; }
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "New password must be at least 6 characters")]
        public string NewPassword { get; set; } = null!;

        [Required]
        [Compare("NewPassword", ErrorMessage = "Password confirmation does not match")]
        public string ConfirmPassword { get; set; } = null!;
    }

    public class ChangeUsernameDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
        public string NewUsername { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }

    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;
    }

    public class SetPasswordDto
    {
        [Required]
        public string Token { get; set; } = null!;

        [Required]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = null!;
    }
}