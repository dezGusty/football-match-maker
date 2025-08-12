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

        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            return await _context.Teams
                .ToListAsync();
        }

        public async Task<Team> GetByIdAsync(int id)
        {
            return await _context.Teams
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Team> GetByNameAsync(string name)
        {
            return await _context.Teams
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Team>> SearchByNameAsync(string searchTerm)
        {
            return await _context.Teams
                .Where(t => t.Name.ToLower().Contains(searchTerm.ToLower()))
                .ToListAsync();
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

        public async Task<bool> DeleteAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
                return false;

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Teams.AnyAsync(t => t.Id == id);
        }
    }
}