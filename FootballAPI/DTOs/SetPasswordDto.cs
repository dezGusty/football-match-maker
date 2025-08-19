using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FootballAPI.DTOs
{
  public class SetPasswordDto
  {
    [Required]
    public string Token { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }
  }
}