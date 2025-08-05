using Microsoft.EntityFrameworkCore;
using FootballAPI.Data;
using FootballAPI.Models;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using static FootballAPI.Repository.UserRepository;

namespace FootballAPI.Repository
{
    public class PlayerRepository : IPlayerRepository
    {
        private FootballDbContext _context;

        public PlayerRepository(FootballDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            return await _context.Players
                .Include(p => p.CurrentTeam)
                .ToListAsync();
        }

        public async Task<Player?> GetByIdAsync(int id)
        {
            return await _context.Players
                .Include(p => p.CurrentTeam)
                .Include(p => p.MatchHistory)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Player>> GetByTeamIdAsync(int teamId)
        {
            return await _context.Players
                .Include(p => p.CurrentTeam)
                .Where(p => p.CurrentTeamId == teamId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Player>> GetEnabledPlayersAsync()
        {
            return await _context.Players
                .Where(p => p.IsEnabled)
                .Include(p => p.CurrentTeam)
                .ToListAsync();
        }

        public async Task<IEnumerable<Player>> GetDisabledPlayersAsync()
        {
            return await _context.Players
                .Where(p => !p.IsEnabled)
                .Include(p => p.CurrentTeam)
                .ToListAsync();
        }
        public async Task<IEnumerable<Player>> GetAvailablePlayersAsync()
        {
            return await _context.Players
                .Include(p => p.CurrentTeam)
                .Where(p => p.IsAvailable && p.IsEnabled)
                .ToListAsync();
        }

        public async Task<Player> CreateAsync(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return player;
        }

        public async Task<Player> UpdateAsync(Player player)
        {
            _context.Entry(player).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return player;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
                return false;

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Players.AnyAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Player>> SearchByNameAsync(string searchTerm)
        {
            return await _context.Players
                .Include(p => p.CurrentTeam)
                .Where(p => p.IsEnabled &&
                   (p.FirstName.Contains(searchTerm) || p.LastName.Contains(searchTerm)))
                .ToListAsync();
        }
        public async Task AddPlayerOrganiserRelationAsync(PlayerOrganiser relation)
        {
            _context.PlayerOrganisers.Add(relation);
            await _context.SaveChangesAsync();
        }
    }
}