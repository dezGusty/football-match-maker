using FootballAPI.Data;
using FootballAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballAPI.Repository
{
    public class MatchTeamsRepository : IMatchTeamsRepository
    {
        private readonly FootballDbContext _context;

        public MatchTeamsRepository(FootballDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MatchTeams>> GetAllAsync()
        {
            return await _context.MatchTeams
                .Include(mt => mt.Match)
                .Include(mt => mt.Team)
                .ToListAsync();
        }

        public async Task<MatchTeams> GetByIdAsync(int id)
        {
            return await _context.MatchTeams
                .Include(mt => mt.Match)
                .Include(mt => mt.Team)
                .FirstOrDefaultAsync(mt => mt.Id == id);
        }

        public async Task<MatchTeams> CreateAsync(MatchTeams matchTeam)
        {
            _context.MatchTeams.Add(matchTeam);
            await _context.SaveChangesAsync();
            return matchTeam;
        }

        public async Task<MatchTeams> UpdateAsync(MatchTeams matchTeam)
        {
            _context.Entry(matchTeam).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return matchTeam;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var matchTeam = await _context.MatchTeams.FindAsync(id);
            if (matchTeam == null)
            {
                return false;
            }

            _context.MatchTeams.Remove(matchTeam);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<MatchTeams>> GetByMatchIdAsync(int matchId)
        {
            return await _context.MatchTeams
                .Include(mt => mt.Team)
                .Include(mt => mt.Match)
                .Where(mt => mt.MatchId == matchId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MatchTeams>> GetByTeamIdAsync(int teamId)
        {
            return await _context.MatchTeams
                .Include(mt => mt.Match)
                .Include(mt => mt.Team)
                .Where(mt => mt.TeamId == teamId)
                .ToListAsync();
        }

        public async Task<MatchTeams> GetByMatchIdAndTeamIdAsync(int matchId, int teamId)
        {
            return await _context.MatchTeams
                .Include(mt => mt.Match)
                .Include(mt => mt.Team)
                .FirstOrDefaultAsync(mt => mt.MatchId == matchId && mt.TeamId == teamId);
        }
    }
}