using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballAPI.Models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrganiserId { get; set; }

        [ForeignKey("OrganiserId")]
        public virtual User Organiser { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public bool IsPublic { get; set; } = false;

        [Required]
        public MatchStatus Status { get; set; } = MatchStatus.Scheduled;

        [Required]
        [Range(10, 12, ErrorMessage = "Max players must be between 10 and 12")]
        public int MaxPlayers { get; set; } = 12;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<MatchPlayer> MatchPlayers { get; set; } = new List<MatchPlayer>();
        public virtual MatchHistory? MatchHistory { get; set; }
    }

    public enum MatchStatus
    {
        Scheduled = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3
    }
}