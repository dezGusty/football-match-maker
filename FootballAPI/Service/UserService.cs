using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository;

namespace FootballAPI.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };
        }

        private UserResponseDto MapToResponseDto(User user, string token = null)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,

            };
        }
        private UserWithImageDto MapToWithImageDto(User user)
        {
            return new UserWithImageDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                ImageUrl = user.ImageUrl
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

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role)
        {
            var users = await _userRepository.GetUsersByRoleAsync(role);
            return users.Select(MapToDto);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            if (await _userRepository.UsernameExistsAsync(createUserDto.Username))
            {
                throw new ArgumentException($"Username '{createUserDto.Username}' already exists.");
            }

            var user = new User
            {
                Username = createUserDto.Username,
                Password = createUserDto.Password,
                Role = createUserDto.Role ?? "User"
            };

            var createdUser = await _userRepository.CreateAsync(user);
            return MapToDto(createdUser);
        }

        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
                return null;

            if (await _userRepository.UsernameExistsAsync(updateUserDto.Username, id))
            {
                throw new ArgumentException($"Username '{updateUserDto.Username}' already exists.");
            }

            existingUser.Username = updateUserDto.Username;
            existingUser.Role = updateUserDto.Role ?? existingUser.Role;

            var updatedUser = await _userRepository.UpdateAsync(existingUser);
            return MapToDto(updatedUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<UserResponseDto> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _userRepository.AuthenticateAsync(loginDto.Username, loginDto.Password);

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

        public async Task<UserWithImageDto> GetUserWithImageByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : MapToWithImageDto(user);
        }

        public async Task<IEnumerable<UserWithImageDto>> GetAllUsersWithImageAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToWithImageDto);
        }

        public async Task<UserWithImageDto> GetUserWithImageByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            return user == null ? null : MapToWithImageDto(user);
        }
        public async Task<IEnumerable<UserWithImageDto>> GetUsersWithImageByRoleAsync(string role)
        {
            var users = await _userRepository.GetUsersByRoleAsync(role);
            return users.Select(MapToWithImageDto);
        }
        public async Task<UserDto?> UpdateUserProfileImageAsync(int id, string imageUrl)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            user.ImageUrl = imageUrl;
            var updatedUser = await _userRepository.UpdateAsync(user);
            return MapToDto(updatedUser);
        }
    }
}