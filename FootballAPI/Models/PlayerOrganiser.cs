using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballAPI.Models
{
    public class PlayerOrganiser
    {
        public int OrganiserId { get; set; }
        [ForeignKey("OrganiserId")]
        public virtual User Organiser { get; set; }
        
        public int PlayerId { get; set; }
        [ForeignKey("PlayerId")]
        public virtual User Player { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}