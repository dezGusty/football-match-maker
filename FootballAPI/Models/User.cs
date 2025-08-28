using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FootballAPI.Models.Enums;

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

        [Required]
        public UserRole Role { get; set; }

        // Properties moved from Player model
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Range(0.0f, 10.0f, ErrorMessage = "Rating must be between 0.0 and 10.0")]
        public float Rating { get; set; } = 0.0f;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; }

        [Range(1, 4, ErrorMessage = "Speed must be between 1 (Low) and 4 (Extreme)")]
        public int Speed { get; set; } = 2;

        [Range(1, 4, ErrorMessage = "Stamina must be between 1 (Low) and 4 (Extreme)")]
        public int Stamina { get; set; } = 2;

        [Range(1, 4, ErrorMessage = "Errors must be between 1 (Low) and 4 (Extreme)")]
        public int Errors { get; set; } = 2; 

        [StringLength(500)]
        public string? ProfileImagePath { get; set; }

        // Delegation fields
        public int? DelegatedToUserId { get; set; }
        [ForeignKey("DelegatedToUserId")]
        public virtual User? DelegatedToUser { get; set; }

        public bool IsDelegatingOrganizer { get; set; } = false;

        public bool IsDelegated { get; set; } = false;

        // Navigation properties
        public virtual ICollection<FriendRequest> SentFriendRequests { get; set; } = new List<FriendRequest>();
        public virtual ICollection<FriendRequest> ReceivedFriendRequests { get; set; } = new List<FriendRequest>();
        public virtual ICollection<ResetPasswordToken> ResetPasswordTokens { get; set; } = new List<ResetPasswordToken>();
        public virtual ICollection<PlayerOrganiser> OrganisedPlayers { get; set; } = new List<PlayerOrganiser>();
        public virtual ICollection<PlayerOrganiser> PlayerRelations { get; set; } = new List<PlayerOrganiser>();
        public virtual ICollection<Match> OrganisedMatches { get; set; } = new List<Match>();
        public virtual ICollection<TeamPlayers> TeamPlayers { get; set; } = new List<TeamPlayers>();
        public virtual ICollection<OrganizerDelegate> OriginalDelegations { get; set; } = new List<OrganizerDelegate>();
        public virtual ICollection<OrganizerDelegate> ReceivedDelegations { get; set; } = new List<OrganizerDelegate>();
    }
}
