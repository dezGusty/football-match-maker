using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository;

namespace FootballAPI.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPlayerRepository _playerRepository;

        public UserService(IUserRepository userRepository, IPlayerRepository playerRepository)
        {
            _userRepository = userRepository;
            _playerRepository = playerRepository;
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
                Password = dto.Password,
                Role = dto.Role,
                ImageUrl = dto.ImageUrl
            };


            var createdUser = await _userRepository.CreateAsync(user);

            return MapToDto(createdUser);
        }

        public async Task<UserDto> CreatePlayerUserAsync(CreatePlayerUserDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                Username = dto.Username,
                Password = dto.Password,
                Role = UserRole.PLAYER,
                ImageUrl = dto.ImageUrl
            };

            var createdUser = await _userRepository.CreateAsync(user);

            var player = new Player
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Rating = dto.Rating,
                Email = dto.Email,
                IsAvailable = false,
                IsEnabled = true,
                ImageUrl = dto.ImageUrl
            };
            await _playerRepository.CreateAsync(player);

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
        public async Task<IEnumerable<UserWithImageDto>> GetUsersWithImageByRoleAsync(UserRole role)
        {
            var users = await _userRepository.GetAllAsync();
            return users.Where(u => u.Role == role).Select(MapToWithImageDto);
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

        public async Task<bool> UpdateUserPasswordAsync(string email)
        {
            var _emailService = new EmailService();

            // Caută user-ul în DB
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                Console.WriteLine($"[ERROR] Nu există user cu email: {email}");
                return false;
            }

            var newPassword = _emailService.GenerateRandomPassword();
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                Console.WriteLine("[ERROR] Parola generată este goală!");
                return false;
            }

            user.Password = newPassword;
            await _userRepository.UpdateAsync(user);

            return await _emailService.SendForgottenPasswordEmailAsync(
                email,
                user.Username ?? "User",
                newPassword
            );
        }




        public async Task<bool> ChangeUsernameAsync(int userId, ChangeUsernameDto changeUsernameDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Verifică dacă parola este corectă
            if (user.Password != changeUsernameDto.Password)
                throw new ArgumentException("Password is incorrect.");

            // Verifică dacă username-ul nou există deja
            if (await _userRepository.UsernameExistsAsync(changeUsernameDto.NewUsername, userId))
                throw new ArgumentException($"Username '{changeUsernameDto.NewUsername}' already exists.");

            return await _userRepository.ChangeUsernameAsync(userId, changeUsernameDto.NewUsername);
        }

    }
}