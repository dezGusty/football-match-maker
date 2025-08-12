using System.ComponentModel.DataAnnotations;

namespace FootballAPI.DTOs
{
    public class PlayerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Range(0.0f, 10000.0f, ErrorMessage = "Rating must be between 0.0 and 10000.0")]
        public float Rating { get; set; }

        public string Email { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsPublic { get; set; }
        public bool IsEnabled { get; set; }

        [Range(1, 3, ErrorMessage = "Speed must be between 1 (Low) and 3 (High)")]
        public int Speed { get; set; }

        [Range(1, 3, ErrorMessage = "Stamina must be between 1 (Low) and 3 (High)")]
        public int Stamina { get; set; }

        [Range(1, 3, ErrorMessage = "Errors must be between 1 (Low) and 3 (High)")]
        public int Errors { get; set; }
    }



    public class CreatePlayerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public float Rating { get; set; }
        public string Email { get; set; }

        [Range(1, 3, ErrorMessage = "Speed must be between 1 (Low) and 3 (High)")]
        public int Speed { get; set; } = 2;

        [Range(1, 3, ErrorMessage = "Stamina must be between 1 (Low) and 3 (High)")]
        public int Stamina { get; set; } = 2;

        [Range(1, 3, ErrorMessage = "Errors must be between 1 (Low) and 3 (High)")]
        public int Errors { get; set; } = 2;

    }

    public class UpdatePlayerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public float Rating { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsPublic { get; set; }

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