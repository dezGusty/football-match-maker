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

        public async Task<IEnumerable<TeamPlayers>> GetAllAsync()
        {
            return await _context.TeamPlayers
                .Include(tp => tp.MatchTeam)
                    .ThenInclude(mt => mt.Match)
                .Include(tp => tp.MatchTeam)
                    .ThenInclude(mt => mt.Team)
                .Include(tp => tp.Player)
                .ToListAsync();
        }

        public async Task<TeamPlayers> GetByIdAsync(int id)
        {
            return await _context.TeamPlayers
                .Include(tp => tp.MatchTeam)
                    .ThenInclude(mt => mt.Match)
                .Include(tp => tp.MatchTeam)
                    .ThenInclude(mt => mt.Team)
                .Include(tp => tp.Player)
                .FirstOrDefaultAsync(tp => tp.Id == id);
        }

        public async Task<TeamPlayers> CreateAsync(TeamPlayers teamPlayer)
        {
            _context.TeamPlayers.Add(teamPlayer);
            await _context.SaveChangesAsync();
            return teamPlayer;
        }

        public async Task<TeamPlayers> UpdateAsync(TeamPlayers teamPlayer)
        {
            _context.Entry(teamPlayer).State = EntityState.Modified;
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

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.TeamPlayers.AnyAsync(tp => tp.Id == id);
        }

        public async Task<IEnumerable<TeamPlayers>> GetByMatchTeamIdAsync(int matchTeamId)
        {
            return await _context.TeamPlayers
                .Include(tp => tp.Player)
                    .ThenInclude(p => p.User)
                .Where(tp => tp.MatchTeamId == matchTeamId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TeamPlayers>> GetByPlayerIdAsync(int playerId)
        {
            return await _context.TeamPlayers
                .Include(tp => tp.MatchTeam)
                    .ThenInclude(mt => mt.Match)
                .Include(tp => tp.MatchTeam)
                    .ThenInclude(mt => mt.Team)
                .Where(tp => tp.PlayerId == playerId)
                .ToListAsync();
        }

        public async Task<TeamPlayers> GetByMatchTeamIdAndPlayerIdAsync(int matchTeamId, int playerId)
        {
            return await _context.TeamPlayers
                .Include(tp => tp.MatchTeam)
                    .ThenInclude(mt => mt.Match)
                .Include(tp => tp.MatchTeam)
                    .ThenInclude(mt => mt.Team)
                .Include(tp => tp.Player)
                .FirstOrDefaultAsync(tp => tp.MatchTeamId == matchTeamId && tp.PlayerId == playerId);
        }

        public async Task<IEnumerable<TeamPlayers>> GetByStatusAsync(PlayerStatus status)
        {
            return await _context.TeamPlayers
                .Include(tp => tp.MatchTeam)
                    .ThenInclude(mt => mt.Match)
                .Include(tp => tp.MatchTeam)
                    .ThenInclude(mt => mt.Team)
                .Include(tp => tp.Player)
                .Where(tp => tp.Status == status)
                .ToListAsync();
        }
    }
}