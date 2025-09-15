using FootballAPI.Data;
using FootballAPI.Models;
using FootballAPI.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace FootballAPI.Repository
{
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private readonly FootballDbContext _context;

        public FriendRequestRepository(FootballDbContext context)
        {
            _context = context;
        }

        public async Task<FriendRequest> CreateAsync(FriendRequest friendRequest)
        {
            _context.FriendRequests.Add(friendRequest);
            await _context.SaveChangesAsync();
            return friendRequest;
        }

        public async Task<FriendRequest?> GetByIdAsync(int id)
        {
            return await _context.FriendRequests
                .Include(fr => fr.Sender)
                .Include(fr => fr.Receiver)
                .FirstOrDefaultAsync(fr => fr.Id == id);
        }

        public async Task<IEnumerable<FriendRequest>> GetSentRequestsAsync(int senderId)
        {
            return await _context.FriendRequests
                .Include(fr => fr.Sender)
                .Include(fr => fr.Receiver)
                .Where(fr => fr.SenderId == senderId)
                .OrderByDescending(fr => fr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<FriendRequest>> GetReceivedRequestsAsync(int receiverId)
        {
            return await _context.FriendRequests
                .Include(fr => fr.Sender)
                .Include(fr => fr.Receiver)
                .Where(fr => fr.ReceiverId == receiverId)
                .OrderByDescending(fr => fr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<FriendRequest>> GetPendingRequestsBetweenUsersAsync(int user1Id, int user2Id)
        {
            return await _context.FriendRequests
                .Where(fr => fr.Status == FriendRequestStatus.Pending &&
                           ((fr.SenderId == user1Id && fr.ReceiverId == user2Id) ||
                            (fr.SenderId == user2Id && fr.ReceiverId == user1Id)))
                .ToListAsync();
        }

        public async Task<FriendRequest> UpdateAsync(FriendRequest friendRequest)
        {
            _context.FriendRequests.Update(friendRequest);
            await _context.SaveChangesAsync();
            return friendRequest;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var friendRequest = await _context.FriendRequests.FindAsync(id);
            if (friendRequest == null)
                return false;

            _context.FriendRequests.Remove(friendRequest);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsPendingRequestAsync(int senderId, int receiverId)
        {
            return await _context.FriendRequests
                .AnyAsync(fr => fr.SenderId == senderId &&
                               fr.ReceiverId == receiverId &&
                               fr.Status == FriendRequestStatus.Pending);
        }

        public async Task<IEnumerable<User>> GetFriendsFromPlayerOrganiserAsync(int userId)
        {
            // Get all accepted friend requests for this user
            var friendRequests = await _context.FriendRequests
                .Where(fr => fr.Status == FriendRequestStatus.Accepted &&
                           (fr.SenderId == userId || fr.ReceiverId == userId))
                .Include(fr => fr.Sender)
                .Include(fr => fr.Receiver)
                .ToListAsync();

            // Extract the friend users and remove duplicates
            var friends = friendRequests
                .Select(fr => fr.SenderId == userId ? fr.Receiver : fr.Sender)
                .Where(user => user != null)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .OrderBy(u => u.Username)
                .ToList();

            return friends;
        }

        public async Task<IEnumerable<FriendRequest>> GetAcceptedFriendsAsync(int userId)
        {
            return await _context.FriendRequests
                .Include(fr => fr.Sender)
                .Include(fr => fr.Receiver)
                .Where(fr => (fr.SenderId == userId || fr.ReceiverId == userId) && 
                            fr.Status == FriendRequestStatus.Accepted)
                .ToListAsync();
        }
    }
}
