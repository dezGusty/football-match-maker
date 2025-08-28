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

        private UserResponseDto MapToResponseDto(User user, string token = null!)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,

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
            };

            var createdUser = await _userRepository.CreateAsync(user);

            // Player properties are now part of User model

            return MapToDto(createdUser);
        }

        public async Task<UserDto> CreatePlayerUserAsync(CreatePlayerUserDto dto, int? organizerId = null)
        {
            if (organizerId.HasValue)
            {
                var organizer = await _userRepository.GetByIdAsync(organizerId.Value);
                if (organizer == null)
                    throw new ArgumentException("Organizer not found");

                if (organizer.Role != UserRole.ORGANISER)
                    throw new ArgumentException("Only organizers can create player users");
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

        public async Task<UserResponseDto> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _userRepository.AuthenticateAsync(loginDto.Email, loginDto.Password);

            if (user == null)
                return null!;

            return MapToResponseDto(user);
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            if (user.Password != changePasswordDto.CurrentPassword)
                throw new ArgumentException("Current password is incorrect.");

            return await _userRepository.ChangePasswordAsync(userId, changePasswordDto.NewPassword);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _userRepository.UsernameExistsAsync(username);
        }

        public async Task<bool> UsernameExistsAsync(string username, int excludeUserId)
        {
            return await _userRepository.UsernameExistsAsync(username, excludeUserId);
        }


        public async Task<bool> ChangeUsernameAsync(int userId, ChangeUsernameDto changeUsernameDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            if (user.Password != changeUsernameDto.Password)
                throw new ArgumentException("Password is incorrect.");

            if (await _userRepository.UsernameExistsAsync(changeUsernameDto.NewUsername, userId))
                throw new ArgumentException($"Username '{changeUsernameDto.NewUsername}' already exists.");

            return await _userRepository.ChangeUsernameAsync(userId, changeUsernameDto.NewUsername);
        }

        public async Task<IEnumerable<User>> GetPlayersByOrganiserAsync(int id)
        {
            return await _userRepository.GetPlayersByOrganiserAsync(id);
        }

        // Player functionality integrated
        public async Task<bool> UpdatePlayerRatingAsync(int userId, float ratingChange)
        {
            return await _userRepository.UpdatePlayerRatingAsync(userId, ratingChange);
        }

        public async Task<string> UpdatePlayerProfileImageAsync(int userId, IFormFile imageFile)
        {
            return await _userRepository.UpdatePlayerProfileImageAsync(userId, imageFile);
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
                var relation = new PlayerOrganiser
                {
                    OrganiserId = organiserId,
                    PlayerId = userId,
                    CreatedAt = DateTime.Now
                };

                await _userRepository.AddPlayerOrganiserRelationAsync(relation);
            }
        }

        public async Task<OrganizerDelegateDto> DelegateOrganizerRoleAsync(int organizerId, DelegateOrganizerRoleDto dto)
        {
            var organizer = await _userRepository.GetByIdAsync(organizerId);
            if (organizer == null || organizer.Role != UserRole.ORGANISER)
                throw new ArgumentException("User is not an organizer");

            var existingDelegation = await _userRepository.GetActiveDelegationByOrganizerId(organizerId);
            if (existingDelegation != null)
                throw new InvalidOperationException("User already has an active delegation");

            var friend = await _userRepository.GetByIdAsync(dto.FriendUserId);
            if (friend == null || friend.Role != UserRole.PLAYER)
                throw new ArgumentException("Friend must be a player");

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

            await _userRepository.TransferPlayerOrganiserRelationsAsync(organizerId, dto.FriendUserId);

            await _userRepository.UpdateUserRoleAndDelegationStatus(organizerId, UserRole.PLAYER, true, dto.FriendUserId);
            await _userRepository.UpdateUserRoleAndDelegationStatus(dto.FriendUserId, UserRole.ORGANISER, false, null);

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

            await _userRepository.TransferPlayerOrganiserRelationsAsync(delegation.DelegateUserId, organizerId);

            await _userRepository.UpdateUserRoleAndDelegationStatus(organizerId, UserRole.ORGANISER, false, null);
            await _userRepository.UpdateUserRoleAndDelegationStatus(delegation.DelegateUserId, UserRole.PLAYER, false, null);

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