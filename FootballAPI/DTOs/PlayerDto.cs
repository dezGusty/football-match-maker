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
        public int? CurrentTeamId { get; set; }
        public bool IsEnabled { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class CreatePlayerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public float Rating { get; set; }
        public string Email { get; set; }
        public string? ImageUrl { get; set; }
        //public int? CurrentTeamId { get; set; }
    }

    public class UpdatePlayerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public float Rating { get; set; }
        public bool IsAvailable { get; set; }
        public int? CurrentTeamId { get; set; }
        public bool IsEnabled { get; set; }
        public string? ImageUrl { get; set; }
    }
}