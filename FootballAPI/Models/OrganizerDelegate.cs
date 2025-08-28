using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballAPI.Models
{
    public class OrganizerDelegate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OriginalOrganizerId { get; set; }
        [ForeignKey("OriginalOrganizerId")]
        public virtual User OriginalOrganizer { get; set; }

        [Required]
        public int DelegateUserId { get; set; }
        [ForeignKey("DelegateUserId")]
        public virtual User DelegateUser { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReclaimedAt { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}