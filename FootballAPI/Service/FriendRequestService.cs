using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Models.Enums;
using FootballAPI.Repository;

namespace FootballAPI.Service
{
    public class FriendRequestService : IFriendRequestService
    {
        private readonly IFriendRequestRepository _friendRequestRepository;
        private readonly IUserRepository _userRepository;

        public FriendRequestService(
            IFriendRequestRepository friendRequestRepository,
            IUserRepository userRepository)
        {
            _friendRequestRepository = friendRequestRepository;
            _userRepository = userRepository;
        }

        private FriendRequestDto MapToDto(FriendRequest friendRequest)
        {
            return new FriendRequestDto
            {
                Id = friendRequest.Id,
                SenderId = friendRequest.SenderId,
                SenderUsername = friendRequest.Sender?.Username ?? "",
                SenderEmail = friendRequest.Sender?.Email ?? "",
                ReceiverId = friendRequest.ReceiverId,
                ReceiverUsername = friendRequest.Receiver?.Username ?? "",
                ReceiverEmail = friendRequest.Receiver?.Email ?? "",
                Status = friendRequest.Status.ToString(),
                CreatedAt = friendRequest.CreatedAt,
                ResponsedAt = friendRequest.ResponsedAt
            };
        }

        public async Task<FriendRequestDto> SendFriendRequestAsync(int senderId, CreateFriendRequestDto dto)
        {
            var sender = await _userRepository.GetByIdAsync(senderId);
            var receiver = await _userRepository.GetByEmailAsync(dto.ReceiverEmail);

            if (sender == null || receiver == null)
                throw new ArgumentException("Sender or receiver not found");

            if (sender.Email == dto.ReceiverEmail)
                throw new ArgumentException("Cannot send friend request to yourself");

            var existingRequests = await _friendRequestRepository.GetPendingRequestsBetweenUsersAsync(senderId, receiver.Id);
            if (existingRequests.Any())
                throw new ArgumentException("A pending friend request already exists between these users");

            // Friend requests are now allowed between all role combinations

            var friendRequest = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiver.Id,
                Status = FriendRequestStatus.Pending,
                CreatedAt = DateTime.Now
            };

            var createdRequest = await _friendRequestRepository.CreateAsync(friendRequest);
            return MapToDto(createdRequest);
        }

        public async Task<FriendRequestDto> RespondToFriendRequestAsync(int requestId, int userId, FriendRequestResponseDto response)
        {
            var friendRequest = await _friendRequestRepository.GetByIdAsync(requestId);

            if (friendRequest == null)
                throw new ArgumentException("Friend request not found");

            if (friendRequest.ReceiverId != userId)
                throw new ArgumentException("You can only respond to requests sent to you");

            if (friendRequest.Status != FriendRequestStatus.Pending)
                throw new ArgumentException("This request has already been responded to");

            friendRequest.Status = response.Accept ? FriendRequestStatus.Accepted : FriendRequestStatus.Rejected;
            friendRequest.ResponsedAt = DateTime.Now;

            var updatedRequest = await _friendRequestRepository.UpdateAsync(friendRequest);

            // Friend request is now accepted - no need to create separate PlayerOrganiser relation
            // The accepted FriendRequest itself represents the friendship

            return MapToDto(updatedRequest);
        }


        public async Task<IEnumerable<FriendRequestDto>> GetSentRequestsAsync(int userId)
        {
            var requests = await _friendRequestRepository.GetSentRequestsAsync(userId);
            return requests.Select(MapToDto);
        }

        public async Task<IEnumerable<FriendRequestDto>> GetReceivedRequestsAsync(int userId)
        {
            var requests = await _friendRequestRepository.GetReceivedRequestsAsync(userId);
            return requests.Select(MapToDto);
        }

        public async Task<IEnumerable<FriendRequestDto>> GetFriendsAsync(int userId)
        {
            var friends = await _friendRequestRepository.GetFriendsFromPlayerOrganiserAsync(userId);
            return friends.Select(user => new FriendRequestDto
            {
                Id = 0,
                SenderId = userId,
                SenderUsername = "",
                SenderEmail = "",
                ReceiverId = user.Id,
                ReceiverUsername = user.Username,
                ReceiverEmail = user.Email,
                Status = "Connected",
                CreatedAt = DateTime.Now,
                ResponsedAt = DateTime.Now
            });
        }

        public async Task<bool> DeleteFriendRequestAsync(int requestId, int userId)
        {
            var friendRequest = await _friendRequestRepository.GetByIdAsync(requestId);

            if (friendRequest == null)
                return false;

            if (friendRequest.SenderId != userId)
                throw new ArgumentException("You can only delete requests you sent");

            return await _friendRequestRepository.DeleteAsync(requestId);
        }

        public async Task<bool> UnfriendAsync(int userId, int friendId)
        {
            var relation = await _userRepository.GetFriendRequestRelationAsync(userId, friendId);

            if (relation == null)
                return false;

            await _userRepository.DeleteFriendRequestRelationAsync(relation);
            return true;
        }

    }
}
