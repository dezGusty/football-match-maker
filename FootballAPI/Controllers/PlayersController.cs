using Microsoft.AspNetCore.Mvc;
using FootballAPI.Models;

namespace Players.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly List<Player> players = new List<Player>
         {
             new Player { Id = 1, FirstName = "John", LastName = "Doe", Rating = 4.5f, IsAvailable = true },
             new Player { Id = 2, FirstName = "Jane", LastName = "Smith", Rating = 3.8f, IsAvailable = false },
             new Player { Id = 3, FirstName = "Mike", LastName = "Johnson", Rating = 4.0f, IsAvailable = true }
         };
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(players);
        }
    }
}