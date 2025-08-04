using FootballAPI.DTOs;
using FootballAPI.Models;

namespace FootballAPI.Service
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IEnumerable<UserWithImageDto>> GetAllUsersWithImageAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserWithImageDto> GetUserWithImageByIdAsync(int id);
        Task<UserWithImageDto> GetUserWithImageByUsernameAsync(string username);
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto> CreatePlayerUserAsync(CreatePlayerUserDto dto);
        Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
        Task<UserResponseDto> AuthenticateAsync(LoginDto loginDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(UserRole role);
        Task<IEnumerable<UserWithImageDto>> GetUsersWithImageByRoleAsync(UserRole role);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> UsernameExistsAsync(string username, int excludeUserId);
        Task<UserDto?> UpdateUserProfileImageAsync(int id, string imageUrl);
        Task<bool> UpdateUserPasswordAsync(string email);

        Task<bool> ChangeUsernameAsync(int userId, ChangeUsernameDto changeUsernameDto);

    }


}