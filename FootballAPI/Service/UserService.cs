using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Models.Enums;
using FootballAPI.Repository;

namespace FootballAPI.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFriendRequestRepository _friendRequestRepository;

        public UserService(IUserRepository userRepository, IFriendRequestRepository friendRequestRepository)
        {
            _userRepository = userRepository;
            _friendRequestRepository = friendRequestRepository;
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Rating = user.Rating,
                IsDeleted = user.DeletedAt.HasValue,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                DeletedAt = user.DeletedAt,
                Speed = user.Speed,
                Stamina = user.Stamina,
                Errors = user.Errors,
                ProfileImageUrl = user.ProfileImagePath
            };
        }


        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null! : MapToDto(user);
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            return user == null ? null! : MapToDto(user);
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user == null ? null : MapToDto(user);
        }
        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(UserRole role)
        {
            var users = await _userRepository.GetUsersByRoleAsync(role);
            return users.Select(MapToDto);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            Console.WriteLine(dto);
            var user = new User
            {
                Email = dto.Email,
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 10),
                Role = dto.Role,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Rating = dto.Rating,
                Speed = dto.Speed,
                Stamina = dto.Stamina,
                Errors = dto.Errors,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateAsync(user);

            return MapToDto(createdUser);
        }

        public async Task<UserDto> CreatePlayerUserAsync(CreatePlayerUserDto dto, int? organizerId = null)
        {
            if (organizerId.HasValue)
            {
                var organizer = await _userRepository.GetByIdAsync(organizerId.Value);
                if (organizer == null)
                    throw new ArgumentException("Organizer not found");

                if (organizer.Role != UserRole.ORGANISER && organizer.Role != UserRole.ADMIN)
                    throw new ArgumentException("Only organizers and admins can create player users");
            }

            if (await _userRepository.UsernameExistsAsync(dto.Username))
            {
                throw new ArgumentException("Username already exists. Please choose a different username.");
            }

            var user = new User
            {
                Email = dto.Email,
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 10),
                Role = dto.Role,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Rating = dto.Rating,
                Speed = dto.Speed,
                Stamina = dto.Stamina,
                Errors = dto.Errors,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateAsync(user);

            return MapToDto(createdUser);
        }


        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);

            if (await _userRepository.UsernameExistsAsync(updateUserDto.Username, id))
            {
                throw new ArgumentException($"Username '{updateUserDto.Username}' already exists.");
            }

            existingUser.Username = updateUserDto.Username;
            existingUser.Role = updateUserDto.Role;
            existingUser.Email = updateUserDto.Email;
            existingUser.Rating = updateUserDto.Rating;
            existingUser.FirstName = updateUserDto.FirstName;
            existingUser.LastName = updateUserDto.LastName;
            existingUser.Speed = updateUserDto.Speed;
            existingUser.Errors = updateUserDto.Errors;
            existingUser.Stamina = updateUserDto.Stamina;

            var updatedUser = await _userRepository.UpdateAsync(existingUser);
            return MapToDto(updatedUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<bool> ReactivateUserAsync(int id)
        {
            return await _userRepository.ReactivateAsync(id);
        }

        public async Task<IEnumerable<User>> GetPlayersByOrganiserAsync(int id)
        {
            return await _userRepository.GetPlayersByOrganiserAsync(id);
        }

        public async Task<bool> UpdatePlayerRatingAsync(int userId, float ratingChange)
        {
            return await _userRepository.UpdatePlayerRatingAsync(userId, ratingChange);
        }

        public async Task<bool> UpdateMultiplePlayerRatingsAsync(List<PlayerRatingUpdateDto> playerRatingUpdates)
        {
            return await _userRepository.UpdateMultiplePlayerRatingsAsync(playerRatingUpdates);
        }

        public async Task<IEnumerable<UserDto>> GetPlayersAsync()
        {
            var users = await _userRepository.GetUsersByRoleAsync(UserRole.PLAYER);
            return users.Select(MapToDto);
        }

        public async Task AddPlayerOrganiserRelationAsync(int userId, int organiserId)
        {
            var organizer = await _userRepository.GetByIdAsync(organiserId);
            var user = await _userRepository.GetByIdAsync(userId);

            if (organizer != null && user != null)
            {
                await _userRepository.AddPlayerOrganiserRelationAsync(organiserId, userId);
            }
        }

        public async Task<OrganizerDelegateDto> DelegateOrganizerRoleAsync(int organizerId, DelegateOrganizerRoleDto dto)
        {
            var organizer = await _userRepository.GetByIdAsync(organizerId);
            if (organizer == null)
                throw new ArgumentException("Organizer not found");

            var existingDelegation = await _userRepository.GetActiveDelegationByOrganizerId(organizerId);
            if (existingDelegation != null)
                throw new InvalidOperationException("User already has an active delegation");

            var friend = await _userRepository.GetByIdAsync(dto.FriendUserId);
            if (friend == null)
                throw new ArgumentException("Friend not found");

            var areFriends = await _userRepository.AreFriends(organizerId, dto.FriendUserId);
            if (!areFriends)
                throw new ArgumentException("Users must be friends to delegate organizer role");

            var friendDelegation = await _userRepository.GetActiveDelegationByDelegateId(dto.FriendUserId);
            if (friendDelegation != null)
                throw new InvalidOperationException("Friend is already acting as a delegate for another organizer");

            var delegation = new OrganizerDelegate
            {
                OriginalOrganizerId = organizerId,
                DelegateUserId = dto.FriendUserId,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createdDelegation = await _userRepository.CreateDelegationAsync(delegation);

            await _userRepository.SwitchOrganizerPlayerRelationAsync(organizerId, dto.FriendUserId);

            await _userRepository.TransferPlayerOrganiserRelationsExcludingAsync(organizerId, dto.FriendUserId, dto.FriendUserId);

            await _userRepository.TransferMatchesAsync(organizerId, dto.FriendUserId);

            await _userRepository.UpdateUserRoleAsync(dto.FriendUserId, UserRole.ORGANISER);

            await _userRepository.UpdateUserRoleAsync(organizerId, UserRole.PLAYER);

            return MapToDelegationDto(createdDelegation, organizer, friend);
        }

        public async Task<bool> ReclaimOrganizerRoleAsync(int organizerId, ReclaimOrganizerRoleDto dto)
        {
            var delegation = await _userRepository.GetActiveDelegationByOrganizerId(organizerId);
            if (delegation == null || delegation.Id != dto.DelegationId)
                return false;

            var success = await _userRepository.ReclaimDelegationAsync(dto.DelegationId, organizerId);
            if (!success)
                return false;

            await _userRepository.TransferPlayerOrganiserRelationsExcludingAsync(delegation.DelegateUserId, organizerId, organizerId);

            await _userRepository.SwitchOrganizerPlayerRelationAsync(delegation.DelegateUserId, organizerId);

            await _userRepository.TransferMatchesAsync(delegation.DelegateUserId, organizerId);

            await _userRepository.UpdateUserRoleAsync(delegation.DelegateUserId, UserRole.PLAYER);

            await _userRepository.UpdateUserRoleAsync(organizerId, UserRole.ORGANISER);

            return true;
        }

        public async Task<DelegationStatusDto> GetDelegationStatusAsync(int userId)
        {
            var currentDelegation = await _userRepository.GetActiveDelegationByOrganizerId(userId);
            var receivedDelegation = await _userRepository.GetActiveDelegationByDelegateId(userId);

            return new DelegationStatusDto
            {
                IsDelegating = currentDelegation != null,
                IsDelegate = receivedDelegation != null,
                CurrentDelegation = currentDelegation != null ? MapToDelegationDto(currentDelegation, currentDelegation.OriginalOrganizer, currentDelegation.DelegateUser) : null,
                ReceivedDelegation = receivedDelegation != null ? MapToDelegationDto(receivedDelegation, receivedDelegation.OriginalOrganizer, receivedDelegation.DelegateUser) : null
            };
        }

        public async Task<bool> IsDelegatedOrganizerAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Role != UserRole.PLAYER)
                return false;

            var currentDelegation = await _userRepository.GetActiveDelegationByOrganizerId(userId);
            return currentDelegation != null;
        }

        public async Task<IEnumerable<UserDto>> GetFriendsAsync(int userId)
        {
            var friendRequests = await _friendRequestRepository.GetAcceptedFriendsAsync(userId);
            var friendIds = friendRequests.Select(fr => fr.SenderId == userId ? fr.ReceiverId : fr.SenderId);

            var friends = new List<User>();
            foreach (var friendId in friendIds)
            {
                var friend = await _userRepository.GetByIdAsync(friendId);
                if (friend != null)
                    friends.Add(friend);
            }

            return friends.Select(MapToDto);
        }

        public async Task<UserDto?> UpdateUserProfileImageAsync(int id, string imageUrl)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            user.ProfileImagePath = imageUrl;
            var updatedUser = await _userRepository.UpdateAsync(user);
            return MapToDto(updatedUser);
        }


        private OrganizerDelegateDto MapToDelegationDto(OrganizerDelegate delegation, User originalOrganizer, User delegateUser)
        {
            return new OrganizerDelegateDto
            {
                Id = delegation.Id,
                OriginalOrganizerId = delegation.OriginalOrganizerId,
                OriginalOrganizerName = $"{originalOrganizer.FirstName} {originalOrganizer.LastName}",
                DelegateUserId = delegation.DelegateUserId,
                DelegateUserName = $"{delegateUser.FirstName} {delegateUser.LastName}",
                CreatedAt = delegation.CreatedAt,
                ReclaimedAt = delegation.ReclaimedAt,
                IsActive = delegation.IsActive,
                Notes = delegation.Notes
            };
        }

    }
}