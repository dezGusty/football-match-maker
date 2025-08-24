using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Models.Enums;
using FootballAPI.Repository;

namespace FootballAPI.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IFriendRequestRepository _friendRequestRepository;

        public UserService(IUserRepository userRepository, IPlayerRepository playerRepository, IFriendRequestRepository friendRequestRepository)
        {
            _userRepository = userRepository;
            _playerRepository = playerRepository;
            _friendRequestRepository = friendRequestRepository;
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                Email = user.Email
            };
        }

        private UserResponseDto MapToResponseDto(User user, string token = null)
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
            return user == null ? null : MapToDto(user);
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            return user == null ? null : MapToDto(user);
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
            var user = new User
            {
                Email = dto.Email,
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 10),
                Role = dto.Role,
            };

            var createdUser = await _userRepository.CreateAsync(user);

            if (dto.Role == UserRole.ORGANISER || dto.Role == UserRole.PLAYER)
            {
                var player = new Player
                {
                    FirstName = dto.Username,
                    LastName = "",
                    Rating = 1000.0f,
                    UserId = createdUser.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _playerRepository.CreateAsync(player);
            }

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

            };

            var createdUser = await _userRepository.CreateAsync(user);

            var player = new Player
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Rating = dto.Rating,
                Speed = dto.Speed,
                Stamina = dto.Stamina,
                Errors = dto.Errors,
                UserId = createdUser.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var createdPlayer = await _playerRepository.CreateAsync(player);

            if (organizerId.HasValue)
            {
                await CreateAutomaticFriendConnectionAsync(organizerId.Value, createdUser.Id);
            }

            return MapToDto(createdUser);
        }

        private async Task CreateAutomaticFriendConnectionAsync(int organizerId, int playerId)
        {
            var friendRequest = new FriendRequest
            {
                SenderId = organizerId,
                ReceiverId = playerId,
                Status = FriendRequestStatus.Accepted,
                CreatedAt = DateTime.Now,
                ResponsedAt = DateTime.Now
            };

            await _friendRequestRepository.CreateAsync(friendRequest);

            var organizer = await _userRepository.GetByIdAsync(organizerId);
            var player = await _userRepository.GetByIdAsync(playerId);

            if (organizer != null && player != null)
            {
                var relation = new PlayerOrganiser
                {
                    OrganiserId = organizerId,
                    PlayerId = playerId,
                    CreatedAt = DateTime.Now
                };

                await _playerRepository.AddPlayerOrganiserRelationAsync(relation);
            }
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
                return null;

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
            var users = await _userRepository.GetPlayersByOrganiserAsync(id);
            return users;
        }
    }
}