using FootballAPI.Data;
using FootballAPI.Models;
using FootballAPI.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace FootballAPI.Repository
{
    public class TeamPlayersRepository : ITeamPlayersRepository
    {
        private readonly FootballDbContext _context;

        public TeamPlayersRepository(FootballDbContext context)
        {
            _context = context;
        }

        public async Task<TeamPlayers> CreateAsync(TeamPlayers teamPlayer)
        {
            _context.TeamPlayers.Add(teamPlayer);
            await _context.SaveChangesAsync();
            return teamPlayer;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var teamPlayer = await _context.TeamPlayers.FindAsync(id);
            if (teamPlayer == null)
            {
                return false;
            }

            _context.TeamPlayers.Remove(teamPlayer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TeamPlayers>> GetByMatchTeamIdAsync(int matchTeamId)
        {
            return await _context.TeamPlayers
                .Include(tp => tp.User)
                    .ThenInclude(u => u.Credentials)
                .Where(tp => tp.MatchTeamId == matchTeamId)
                .ToListAsync();
        }

        public async Task<TeamPlayers> GetByMatchTeamIdAndUserIdAsync(int matchTeamId, int userId)
        {
            return await _context.TeamPlayers
                .Include(tp => tp.MatchTeam)
                    .ThenInclude(mt => mt.Match)
                .Include(tp => tp.MatchTeam)
                    .ThenInclude(mt => mt.Team)
                .Include(tp => tp.User)
                    .ThenInclude(u => u.Credentials)
                .FirstOrDefaultAsync(tp => tp.MatchTeamId == matchTeamId && tp.UserId == userId);
        }
    }
}