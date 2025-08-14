using FootballAPI.DTOs;
using FootballAPI.Models;

namespace FootballAPI.Service
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto> CreatePlayerUserAsync(CreatePlayerUserDto dto);
        Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
        Task<UserResponseDto> AuthenticateAsync(LoginDto loginDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(UserRole role);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> UsernameExistsAsync(string username, int excludeUserId);
        Task<bool> UpdateUserPasswordAsync(string email);

        Task<bool> ChangeUsernameAsync(int userId, ChangeUsernameDto changeUsernameDto);
        Task<IEnumerable<Player>> GetPlayersByOrganiserAsync(int id);


    }


}