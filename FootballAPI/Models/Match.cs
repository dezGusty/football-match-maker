using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public enum Status
{
    Open = 1,
    Closed = 2,
    Finalized = 4,
    Cancelled = 8

}
namespace FootballAPI.Models
{

    public class Match
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime MatchDate { get; set; }

        public bool IsPublic { get; set; } = false;

        public Status Status { get; set; } = Status.Open;

        [StringLength(255)]
        public string? Location { get; set; }

        public decimal? Cost { get; set; }

        [Required]
        public int OrganiserId { get; set; }

        [ForeignKey("OrganiserId")]
        public virtual User Organiser { get; set; }

        public virtual ICollection<MatchTeams> MatchTeams { get; set; } = new List<MatchTeams>();
    }


}
