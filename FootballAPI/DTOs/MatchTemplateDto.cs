using System.ComponentModel.DataAnnotations;

namespace FootballAPI.DTOs
{
    public class MatchTemplateDto
    {
        public int Id { get; set; }
        
        [Required]
        public string Location { get; set; }
        
        [Required]
        public decimal Cost { get; set; }
        
        public string Name { get; set; }
        
        public string TeamAName { get; set; }
        
        public string TeamBName { get; set; }
    }
    
    public class CreateMatchTemplateDto
    {
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
    }
    
    public class UpdateMatchTemplateDto
    {
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
    }
}
