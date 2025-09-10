using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballAPI.Models
{
    public class MatchTemplate
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Location { get; set; }
        
        [Required]
        public decimal Cost { get; set; }
        
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(100)]
        public string TeamAName { get; set; }
        
        [StringLength(100)]
        public string TeamBName { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
