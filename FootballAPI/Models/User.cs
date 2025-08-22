using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        // Navigation properties
        public virtual ICollection<FriendRequest> SentFriendRequests { get; set; } = new List<FriendRequest>();
        public virtual ICollection<FriendRequest> ReceivedFriendRequests { get; set; } = new List<FriendRequest>();
        public virtual ICollection<ResetPasswordToken> ResetPasswordTokens { get; set; } = new List<ResetPasswordToken>();
        public virtual ICollection<PlayerOrganiser> OrganisedPlayers { get; set; } = new List<PlayerOrganiser>();
        public virtual ICollection<Match> OrganisedMatches { get; set; } = new List<Match>();
    }
}
