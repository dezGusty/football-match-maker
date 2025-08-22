using System.ComponentModel.DataAnnotations;

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
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public float Rating { get; set; }
        public string Email { get; set; } = string.Empty; // Still need email to create user

    }

    public class UpdatePlayerDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
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
        public float RatingChange { get; set; }
    }

    public class PlayerRatingUpdateDto
    {
        public int PlayerId { get; set; }
        public float RatingChange { get; set; }
    }

    public class UpdateMultipleRatingsDto
    {
        public List<PlayerRatingUpdateDto> PlayerRatingUpdates { get; set; } = new List<PlayerRatingUpdateDto>();
    }
}