using Microsoft.EntityFrameworkCore;
using FootballAPI.Data;
using FootballAPI.Models;
using FootballAPI.Models.Enums;
using FootballAPI.DTOs;
using Microsoft.AspNetCore.Http;

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
            .Include(po => po.Player)
            .Select(po => po.Player)
            .ToListAsync();

            return users;
        }
        public async Task<User?> GetUserByEmail(string email, bool includeDeleted = false, bool tracking = false)
        {
            IQueryable<User> query = _context.Users;

            if (!tracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Player functionality integrated
        public async Task<bool> UpdatePlayerRatingAsync(int userId, float ratingChange)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            user.Rating += ratingChange;
            user.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(user);
            return true;
        }

        public async Task<string> UpdatePlayerProfileImageAsync(int userId, IFormFile imageFile)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            // This would typically involve file storage logic
            // For now, just updating the path
            var imagePath = $"/images/players/{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}.jpg";
            user.ProfileImagePath = imagePath;
            user.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(user);

            return imagePath;
        }

        public async Task<bool> UpdateMultiplePlayerRatingsAsync(List<PlayerRatingUpdateDto> playerRatingUpdates)
        {
            try
            {
                foreach (var update in playerRatingUpdates)
                {
                    await UpdatePlayerRatingAsync(update.UserId, update.RatingChange);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task AddPlayerOrganiserRelationAsync(PlayerOrganiser relation)
        {
            _context.PlayerOrganisers.Add(relation);
            await _context.SaveChangesAsync();
        }

        // Organizer delegation functionality
        public async Task<OrganizerDelegate> CreateDelegationAsync(OrganizerDelegate delegation)
        {
            _context.OrganizerDelegates.Add(delegation);
            await _context.SaveChangesAsync();
            return delegation;
        }

        public async Task<OrganizerDelegate?> GetActiveDelegationByOrganizerId(int organizerId)
        {
            return await _context.OrganizerDelegates
                .Include(d => d.OriginalOrganizer)
                .Include(d => d.DelegateUser)
                .FirstOrDefaultAsync(d => d.OriginalOrganizerId == organizerId && d.IsActive);
        }

        public async Task<OrganizerDelegate?> GetActiveDelegationByDelegateId(int delegateId)
        {
            return await _context.OrganizerDelegates
                .Include(d => d.OriginalOrganizer)
                .Include(d => d.DelegateUser)
                .FirstOrDefaultAsync(d => d.DelegateUserId == delegateId && d.IsActive);
        }

        public async Task<bool> ReclaimDelegationAsync(int delegationId, int originalOrganizerId)
        {
            var delegation = await _context.OrganizerDelegates
                .FirstOrDefaultAsync(d => d.Id == delegationId && d.OriginalOrganizerId == originalOrganizerId && d.IsActive);

            if (delegation == null)
                return false;

            delegation.IsActive = false;
            delegation.ReclaimedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AreFriends(int userId1, int userId2)
        {
            return await _context.PlayerOrganisers.AnyAsync(po =>
                (po.OrganiserId == userId1 && po.PlayerId == userId2) ||
                 (po.PlayerId == userId2 && po.OrganiserId == userId1));
        }

        public async Task<bool> TransferPlayerOrganiserRelationsAsync(int fromOrganizerId, int toOrganizerId)
        {
            try
            {

                var existingRelations = await _context.PlayerOrganisers
                    .Where(po => po.OrganiserId == fromOrganizerId)
                    .ToListAsync();

                var relationsToTransfer = existingRelations
                    .Where(po => po.PlayerId != fromOrganizerId)
                    .ToList();

                _context.PlayerOrganisers.RemoveRange(existingRelations);

                var newRelations = relationsToTransfer.Select(relation => new PlayerOrganiser
                {
                    OrganiserId = toOrganizerId,
                    PlayerId = relation.PlayerId,
                    CreatedAt = DateTime.Now
                }).ToList();

                await _context.PlayerOrganisers.AddRangeAsync(newRelations);

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RestoreOrganizerPlayerRelationAsync(int organizerId)
        {
            try
            {
                var existingRelation = await _context.PlayerOrganisers
                    .FirstOrDefaultAsync(po => po.OrganiserId == organizerId && po.PlayerId == organizerId);

                if (existingRelation == null)
                {
                    var newRelation = new PlayerOrganiser
                    {
                        OrganiserId = organizerId,
                        PlayerId = organizerId,
                        CreatedAt = DateTime.Now
                    };

                    await _context.PlayerOrganisers.AddAsync(newRelation);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> TransferMatchesAsync(int fromOrganizerId, int toOrganizerId)
        {
            try
            {
                var matchesToTransfer = await _context.Matches
                    .Where(m => m.OrganiserId == fromOrganizerId)
                    .ToListAsync();

                foreach (var match in matchesToTransfer)
                {
                    match.OrganiserId = toOrganizerId;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserRoleAndDelegationStatus(int userId, UserRole newRole, bool isDelegating, int? delegatedToUserId, bool? isDelegated = null)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            user.Role = newRole;
            user.IsDelegatingOrganizer = isDelegating;
            user.DelegatedToUserId = delegatedToUserId;
            if (isDelegated.HasValue)
                user.IsDelegated = isDelegated.Value;
            user.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(user);
            return true;
        }
    }
}