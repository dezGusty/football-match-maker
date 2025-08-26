using System.ComponentModel.DataAnnotations;
using FootballAPI.Models.Enums;

namespace FootballAPI.DTOs
{
    public class PlayerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        
        [Range(0.0f, 10000.0f, ErrorMessage = "Rating must be between 0.0 and 10000.0")]
        public float Rating { get; set; }
        
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        [Range(1, 3, ErrorMessage = "Speed must be between 1 (Low) and 3 (High)")]
        public int Speed { get; set; }

        [Range(1, 3, ErrorMessage = "Stamina must be between 1 (Low) and 3 (High)")]
        public int Stamina { get; set; }

        [Range(1, 3, ErrorMessage = "Errors must be between 1 (Low) and 3 (High)")]
        public int Errors { get; set; }

        public string? ProfileImageUrl { get; set; }
        public string? UserEmail { get; set; }
        public string? Username { get; set; }
    }

    public class CreatePlayerDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [Range(0.0f, 10000.0f, ErrorMessage = "Rating must be between 0.0 and 10000.0")]
        public float Rating { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Range(1, 3, ErrorMessage = "Speed must be between 1 (Low) and 3 (High)")]
        public int Speed { get; set; } = 2;

        [Range(1, 3, ErrorMessage = "Stamina must be between 1 (Low) and 3 (High)")]
        public int Stamina { get; set; } = 2;

        [Range(1, 3, ErrorMessage = "Errors must be between 1 (Low) and 3 (High)")]
        public int Errors { get; set; } = 2;

        [Required]
        public int UserId { get; set; }

    }

    public class CreatePlayerUserDto
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
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [Range(0.0f, 10000.0f, ErrorMessage = "Rating must be between 0.0 and 10000.0")]
        public float Rating { get; set; } = 0.0f;

        [Range(1, 3, ErrorMessage = "Speed must be between 1 (Low) and 3 (High)")]
        public int Speed { get; set; } = 2;

        [Range(1, 3, ErrorMessage = "Stamina must be between 1 (Low) and 3 (High)")]
        public int Stamina { get; set; } = 2;

        [Range(1, 3, ErrorMessage = "Errors must be between 1 (Low) and 3 (High)")]
        public int Errors { get; set; } = 2;

        [Required]
        public UserRole Role { get; set; }
    }

    public class UpdatePlayerDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;
        
        [Range(0.0f, 10000.0f, ErrorMessage = "Rating must be between 0.0 and 10000.0")]
        public float Rating { get; set; }

        [Range(1, 3, ErrorMessage = "Speed must be between 1 (Low) and 3 (High)")]
        public int Speed { get; set; }

        [Range(1, 3, ErrorMessage = "Stamina must be between 1 (Low) and 3 (High)")]
        public int Stamina { get; set; }

        [Range(1, 3, ErrorMessage = "Errors must be between 1 (Low) and 3 (High)")]
        public int Errors { get; set; }
    }

    public class UpdatePlayerRatingDto
    {
        [Range(-10000.0f, 10000.0f, ErrorMessage = "Rating change must be between -10000.0 and 10000.0")]
        public float RatingChange { get; set; }
    }

    public class PlayerRatingUpdateDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PlayerId must be greater than 0")]
        public int PlayerId { get; set; }
        
        [Range(-10000.0f, 10000.0f, ErrorMessage = "Rating change must be between -10000.0 and 10000.0")]
        public float RatingChange { get; set; }
    }

    public class UpdateMultipleRatingsDto
    {
        public List<PlayerRatingUpdateDto> PlayerRatingUpdates { get; set; } = new List<PlayerRatingUpdateDto>();
    }

    public class PlayerOrganiserDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PlayerId must be greater than 0")]
        public int PlayerId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "OrganiserId must be greater than 0")]
        public int OrganiserId { get; set; }
    }

    public class CreatePlayerMatchHistoryDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PlayerId must be greater than 0")]
        public int PlayerId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TeamId must be greater than 0")]
        public int TeamId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "MatchId must be greater than 0")]
        public int MatchId { get; set; }

        [Range(0.0f, 10000.0f, ErrorMessage = "PerformanceRating must be between 0.0 and 10000.0")]
        public float PerformanceRating { get; set; } = 0.0f;
    }

    public class UpdatePlayerMatchHistoryDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PlayerId must be greater than 0")]
        public int PlayerId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TeamId must be greater than 0")]
        public int TeamId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "MatchId must be greater than 0")]
        public int MatchId { get; set; }

        [Range(0.0f, 10000.0f, ErrorMessage = "PerformanceRating must be between 0.0 and 10000.0")]
        public float PerformanceRating { get; set; }
    }
}