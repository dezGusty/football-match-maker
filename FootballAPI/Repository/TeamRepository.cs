using Microsoft.EntityFrameworkCore;
using FootballAPI.Data;
using FootballAPI.Models;

namespace FootballAPI.Repository
{
    public class TeamRepository : ITeamRepository
    {
        private readonly FootballDbContext _context;

        public TeamRepository(FootballDbContext context)
        {
            _context = context;
        }

        public async Task<Team> GetByIdAsync(int id)
        {
            return await _context.Teams
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Team> CreateAsync(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return team;
        }

        public async Task<Team> UpdateAsync(Team team)
        {
            _context.Entry(team).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return team;
        }
    }
}