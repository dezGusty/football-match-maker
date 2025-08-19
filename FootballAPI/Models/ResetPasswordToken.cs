using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballAPI.Models
{
  public class ResetPasswordToken
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(255)]
    public string TokenHash { get; set; } = null!;
    [Required]
    public DateTime ExpiresAt { get; set; }

    public DateTime? UsedAt { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    public bool IsValid()
    {
      return UsedAt == null && DateTime.UtcNow <= ExpiresAt;
    }
    public void MarkAsUsed()
    {
      UsedAt = DateTime.UtcNow;
    }
  }
}