using Microsoft.AspNetCore.Mvc;
using FootballAPI.Models;
using Microsoft.EntityFrameworkCore;
using FootballAPI.Data;

namespace Players.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly FootballDbContext _context;

        public PlayersController(FootballDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<JsonResult> Get()
        {
            var players = await _context.Players.ToListAsync();
            return new JsonResult(players);
        }
    }
}