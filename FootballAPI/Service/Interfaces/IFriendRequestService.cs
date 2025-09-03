using FootballAPI.Models.Enums;
using FootballAPI.DTOs;
using FootballAPI.Models;

namespace FootballAPI.Service
{
    public interface IFriendRequestService
    {
        Task<FriendRequestDto> SendFriendRequestAsync(int senderId, CreateFriendRequestDto dto);
        Task<FriendRequestDto> RespondToFriendRequestAsync(int requestId, int userId, FriendRequestResponseDto response);
        Task<IEnumerable<FriendRequestDto>> GetSentRequestsAsync(int userId);
        Task<IEnumerable<FriendRequestDto>> GetReceivedRequestsAsync(int userId);
        Task<IEnumerable<FriendRequestDto>> GetFriendsAsync(int userId);
        Task<bool> DeleteFriendRequestAsync(int requestId, int userId);
        Task<bool> UnfriendAsync(int userId, int friendId);

    }
}
