using Microsoft.EntityFrameworkCore;
using FootballAPI.Data;
using FootballAPI.Models;
using FootballAPI.Models.Enums;

namespace FootballAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly FootballDbContext _context;

        public UserRepository(FootballDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Set<User>()
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await _context.Set<User>()
                .Where(u => u.Role == role)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Set<User>().Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Set<User>().FindAsync(id);
            if (user == null)
                return false;

            _context.Set<User>().Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Set<User>().AnyAsync(u => u.Id == id);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Set<User>()
                .AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<bool> UsernameExistsAsync(string username, int excludeUserId)
        {
            return await _context.Set<User>()
                .AnyAsync(u => u.Username.ToLower() == username.ToLower() && u.Id != excludeUserId);
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var user = await GetByEmailAsync(email);

            if (user == null || user.Password != password)
                return null;

            return user;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            user.Password = newPassword;
            await UpdateAsync(user);
            return true;
        }


        public async Task<bool> ChangeUsernameAsync(int userId, string newUsername)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            user.Username = newUsername;
            await UpdateAsync(user);
            return true;
        }

        public async Task<IEnumerable<User>> GetPlayersByOrganiserAsync(int id)
        {
            var users = await _context.PlayerOrganisers
            .Where(po => po.OrganiserId == id)
            .Select(po => po.Player)
            .ToListAsync();

            return users;
        }
        public async Task<User?> GetUserByEmail(string email, bool includeDeleted = false, bool tracking = false)
        {
            IQueryable<User> query = _context.Users;

            if (!tracking)
                query = query.AsNoTracking();

            //if (!includeDeleted)
            //    query = query.Where(u => u.DeletedAt == null);

            return await query.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}