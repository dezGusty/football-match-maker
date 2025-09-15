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

            user.DeletedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReactivateAsync(int id)
        {
            var user = await _context.Set<User>().FindAsync(id);
            if (user == null)
                return false;

            user.DeletedAt = null;
            user.UpdatedAt = DateTime.UtcNow;
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
            // Get all friends regardless of role
            var users = await _context.FriendRequests
                .Where(fr => fr.Status == FriendRequestStatus.Accepted &&
                           (fr.SenderId == id || fr.ReceiverId == id))
                .Include(fr => fr.Sender)
                .Include(fr => fr.Receiver)
                .Select(fr => fr.SenderId == id ? fr.Receiver : fr.Sender)
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

            user.Rating = Math.Max(0f, user.Rating + ratingChange);
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

        public async Task AddPlayerOrganiserRelationAsync(int organizerId, int playerId)
        {
            // Check if a friend request already exists between these users
            var existingRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr =>
                    (fr.SenderId == organizerId && fr.ReceiverId == playerId) ||
                    (fr.SenderId == playerId && fr.ReceiverId == organizerId));

            if (existingRequest == null)
            {
                // Create a new accepted friend request
                var friendRequest = new FriendRequest
                {
                    SenderId = organizerId,
                    ReceiverId = playerId,
                    Status = FriendRequestStatus.Accepted,
                    CreatedAt = DateTime.UtcNow,
                    ResponsedAt = DateTime.UtcNow
                };
                _context.FriendRequests.Add(friendRequest);
                await _context.SaveChangesAsync();
            }
            else if (existingRequest.Status != FriendRequestStatus.Accepted)
            {
                // Update existing request to accepted
                existingRequest.Status = FriendRequestStatus.Accepted;
                existingRequest.ResponsedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> RemovePlayerOrganiserRelationAsync(int organizerId, int playerId)
        {
            try
            {
                var friendRequest = await _context.FriendRequests
                    .FirstOrDefaultAsync(fr =>
                        (fr.SenderId == organizerId && fr.ReceiverId == playerId) ||
                        (fr.SenderId == playerId && fr.ReceiverId == organizerId));

                if (friendRequest != null)
                {
                    _context.FriendRequests.Remove(friendRequest);
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
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
            return await _context.FriendRequests.AnyAsync(fr =>
                fr.Status == FriendRequestStatus.Accepted &&
                ((fr.SenderId == userId1 && fr.ReceiverId == userId2) ||
                 (fr.SenderId == userId2 && fr.ReceiverId == userId1)));
        }

        public async Task<bool> TransferPlayerOrganiserRelationsAsync(int fromOrganizerId, int toOrganizerId)
        {
            return await TransferPlayerOrganiserRelationsAsync(fromOrganizerId, toOrganizerId, null);
        }

        public async Task<bool> TransferPlayerOrganiserRelationsAsync(int fromOrganizerId, int toOrganizerId, int? excludePlayerId)
        {
            try
            {
                // Get all accepted friend requests where fromOrganizerId is involved with players
                var existingFriendRequests = await _context.FriendRequests
                    .Where(fr => fr.Status == FriendRequestStatus.Accepted &&
                               ((fr.SenderId == fromOrganizerId && fr.Receiver.Role == UserRole.PLAYER) ||
                                (fr.ReceiverId == fromOrganizerId && fr.Sender.Role == UserRole.PLAYER)))
                    .Include(fr => fr.Sender)
                    .Include(fr => fr.Receiver)
                    .ToListAsync();

                var friendRequestsToTransfer = existingFriendRequests
                    .Where(fr => {
                        var playerId = fr.SenderId == fromOrganizerId ? fr.ReceiverId : fr.SenderId;
                        return playerId != fromOrganizerId && (!excludePlayerId.HasValue || playerId != excludePlayerId.Value);
                    })
                    .ToList();

                // Remove the old friend requests
                var friendRequestsToRemove = excludePlayerId.HasValue
                    ? existingFriendRequests.Where(fr => {
                        var playerId = fr.SenderId == fromOrganizerId ? fr.ReceiverId : fr.SenderId;
                        return playerId != excludePlayerId.Value;
                    }).ToList()
                    : existingFriendRequests;

                _context.FriendRequests.RemoveRange(friendRequestsToRemove);

                // Create new friend requests with the new organizer
                var newFriendRequests = friendRequestsToTransfer.Select(fr => {
                    var playerId = fr.SenderId == fromOrganizerId ? fr.ReceiverId : fr.SenderId;
                    return new FriendRequest
                    {
                        SenderId = toOrganizerId,
                        ReceiverId = playerId,
                        Status = FriendRequestStatus.Accepted,
                        CreatedAt = DateTime.UtcNow,
                        ResponsedAt = DateTime.UtcNow
                    };
                }).ToList();

                await _context.FriendRequests.AddRangeAsync(newFriendRequests);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> TransferPlayerOrganiserRelationsExcludingAsync(int fromOrganizerId, int toOrganizerId, int excludePlayerId)
        {
            return await TransferPlayerOrganiserRelationsAsync(fromOrganizerId, toOrganizerId, excludePlayerId);
        }

        public async Task<bool> SwitchOrganizerPlayerRelationAsync(int originalOrganizerId, int delegatePlayerId)
        {
            try
            {
                var existingFriendRequest = await _context.FriendRequests
                    .FirstOrDefaultAsync(fr => fr.Status == FriendRequestStatus.Accepted &&
                                             ((fr.SenderId == originalOrganizerId && fr.ReceiverId == delegatePlayerId) ||
                                              (fr.SenderId == delegatePlayerId && fr.ReceiverId == originalOrganizerId)));

                if (existingFriendRequest != null)
                {
                    _context.FriendRequests.Remove(existingFriendRequest);

                    var switchedFriendRequest = new FriendRequest
                    {
                        SenderId = delegatePlayerId,
                        ReceiverId = originalOrganizerId,
                        Status = FriendRequestStatus.Accepted,
                        CreatedAt = DateTime.UtcNow,
                        ResponsedAt = DateTime.UtcNow
                    };

                    await _context.FriendRequests.AddAsync(switchedFriendRequest);
                    await _context.SaveChangesAsync();
                }

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
                // This method was creating self-relationships which don't make sense in FriendRequest context
                // In the new system, we don't need self-friend relationships
                // This method can be left empty or removed as it's not needed
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

        public async Task<bool> UpdateUserRoleAsync(int userId, UserRole newRole)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            user.Role = newRole;
            user.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(user);
            return true;
        }

        public async Task<FriendRequest?> GetFriendRequestRelationAsync(int userId, int friendId)
        {
            return await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.Status == FriendRequestStatus.Accepted &&
                    ((fr.SenderId == userId && fr.ReceiverId == friendId) ||
                     (fr.SenderId == friendId && fr.ReceiverId == userId)));
        }

        public async Task DeleteFriendRequestRelationAsync(FriendRequest relation)
        {
            _context.FriendRequests.Remove(relation);
            await _context.SaveChangesAsync();
        }
    }
}