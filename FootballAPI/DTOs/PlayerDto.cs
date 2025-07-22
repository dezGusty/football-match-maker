using System.ComponentModel.DataAnnotations;

namespace FootballAPI.DTOs
{
    public class PlayerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Range(0, 11, ErrorMessage = "Rating must be positive.")]
        public float Rating { get; set; }
        public bool IsAvailable { get; set; }
        public int? CurrentTeamId { get; set; }
        public bool IsEnabled { get; set; }

    }

    public class CreatePlayerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Range(0, 11, ErrorMessage = "Rating must be positive.")]
        public int Rating { get; set; }

    }

    public class UpdatePlayerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Range(0, 11, ErrorMessage = "Rating must be positive.")]
        public float Rating { get; set; }
        public bool IsAvailable { get; set; }
        public int? CurrentTeamId { get; set; }
        public bool IsEnabled { get; set; }
    }
}