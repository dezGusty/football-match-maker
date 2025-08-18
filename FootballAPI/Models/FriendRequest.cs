using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballAPI.Models
{
    public class FriendRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }
        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }

        [Required]
        public int ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }

        [Required]
        public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ResponsedAt { get; set; }
    }

    public enum FriendRequestStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }
}
