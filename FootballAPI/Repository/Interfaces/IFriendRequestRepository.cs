using FootballAPI.Models.Enums;
using FootballAPI.Models;

namespace FootballAPI.Repository
{
    public interface IFriendRequestRepository
    {
        Task<FriendRequest> CreateAsync(FriendRequest friendRequest);
        Task<FriendRequest?> GetByIdAsync(int id);
        Task<IEnumerable<FriendRequest>> GetSentRequestsAsync(int senderId);
        Task<IEnumerable<FriendRequest>> GetReceivedRequestsAsync(int receiverId);
        Task<IEnumerable<User>> GetFriendsFromPlayerOrganiserAsync(int userId);
        Task<IEnumerable<FriendRequest>> GetPendingRequestsBetweenUsersAsync(int user1Id, int user2Id);
        Task<FriendRequest> UpdateAsync(FriendRequest friendRequest);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsPendingRequestAsync(int senderId, int receiverId);
    }
}
