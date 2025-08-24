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
        private readonly IPlayerRepository _playerRepository;

        public FriendRequestService(
            IFriendRequestRepository friendRequestRepository,
            IUserRepository userRepository,
            IPlayerRepository playerRepository)
        {
            _friendRequestRepository = friendRequestRepository;
            _userRepository = userRepository;
            _playerRepository = playerRepository;
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

            if (sender.Role == UserRole.PLAYER && receiver.Role == UserRole.PLAYER)
            {
                throw new ArgumentException("Friend requests between two players are not allowed. At least one must be an organizer.");
            }

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

            if (response.Accept)
            {
                await CreatePlayerOrganiserRelation(friendRequest.SenderId, friendRequest.ReceiverId);
            }

            return MapToDto(updatedRequest);
        }

        private async Task CreatePlayerOrganiserRelation(int senderId, int receiverId)
        {
            var sender = await _userRepository.GetByIdAsync(senderId);
            var receiver = await _userRepository.GetByIdAsync(receiverId);

            if (sender == null || receiver == null)
                throw new ArgumentException("Sender or receiver not found");

            int organiserId, userId;

            if (sender.Role == UserRole.ORGANISER)
            {
                organiserId = senderId;
                userId = receiverId;
            }
            else
            {
                organiserId = receiverId;
                userId = senderId;
            }

            var relation = new PlayerOrganiser
            {
                OrganiserId = organiserId,
                PlayerId = userId,
                CreatedAt = DateTime.Now
            };

            await _playerRepository.AddPlayerOrganiserRelationAsync(relation);
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
            var friendRequests = await _friendRequestRepository.GetFriendsAsync(userId);
            return friendRequests.Select(MapToDto);
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
    }
}
