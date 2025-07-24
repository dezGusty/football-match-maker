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
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
        Task<UserResponseDto> AuthenticateAsync(LoginDto loginDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role);
        Task<IEnumerable<UserWithImageDto>> GetUsersWithImageByRoleAsync(string role);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> UsernameExistsAsync(string username, int excludeUserId);
    }
}