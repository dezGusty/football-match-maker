using FootballAPI.Models.Enums;
using FootballAPI.DTOs;
using FootballAPI.Models;
using Microsoft.AspNetCore.Http;

namespace FootballAPI.Service
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto> CreatePlayerUserAsync(CreatePlayerUserDto dto, int? organizerId = null);
        Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
        Task<UserResponseDto> AuthenticateAsync(LoginDto loginDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(UserRole role);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> UsernameExistsAsync(string username, int excludeUserId);
        Task<bool> ChangeUsernameAsync(int userId, ChangeUsernameDto changeUsernameDto);
        Task<IEnumerable<User>> GetPlayersByOrganiserAsync(int id);
        
        // Player functionality integrated
        Task<bool> UpdatePlayerRatingAsync(int userId, float ratingChange);
        Task<string> UpdatePlayerProfileImageAsync(int userId, IFormFile imageFile);
        Task<bool> UpdateMultiplePlayerRatingsAsync(List<PlayerRatingUpdateDto> playerRatingUpdates);
        Task<IEnumerable<UserDto>> GetPlayersAsync();
        Task AddPlayerOrganiserRelationAsync(int userId, int organiserId);
        
        // Organizer delegation functionality
        Task<OrganizerDelegateDto> DelegateOrganizerRoleAsync(int organizerId, DelegateOrganizerRoleDto dto);
        Task<bool> ReclaimOrganizerRoleAsync(int organizerId, ReclaimOrganizerRoleDto dto);
        Task<DelegationStatusDto> GetDelegationStatusAsync(int userId);
        Task<IEnumerable<UserDto>> GetFriendsAsync(int userId);

    }

}