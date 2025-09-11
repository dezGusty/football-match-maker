using System;
using System.ComponentModel.DataAnnotations;

namespace FootballAPI.Models
{
    public class ImpersonationLog
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int AdminId { get; set; }
        
        [Required]
        public int ImpersonatedUserId { get; set; }
        
        [Required]
        public DateTime StartTime { get; set; }
        
        public DateTime? EndTime { get; set; }
        
        // Navigation properties
        public virtual User Admin { get; set; }
        public virtual User ImpersonatedUser { get; set; }
    }
}
