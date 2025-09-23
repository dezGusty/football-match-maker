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
        Task<UserDto?> UpdatePlayerAsync(int id, UpdatePlayerDto updatePlayerDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ReactivateUserAsync(int id);
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(UserRole role);
        Task<IEnumerable<User>> GetPlayersByOrganiserAsync(int id);

        Task<bool> UpdatePlayerRatingAsync(int userId, float newRating,
            string changeReason = "Manual", int? matchId = null, string? ratingSystem = null);
        Task<bool> UpdateMultiplePlayerRatingsAsync(List<PlayerRatingUpdateDto> playerRatingUpdates);
        Task<IEnumerable<UserDto>> GetPlayersAsync();
        Task AddPlayerOrganiserRelationAsync(int userId, int organiserId);
        Task<UserDto?> UpdateUserProfileImageAsync(int id, string? imageUrl);

        Task<OrganizerDelegateDto> DelegateOrganizerRoleAsync(int organizerId, DelegateOrganizerRoleDto dto);
        Task<bool> ReclaimOrganizerRoleAsync(int organizerId, ReclaimOrganizerRoleDto dto);
        Task<DelegationStatusDto> GetDelegationStatusAsync(int userId);
        Task<bool> IsDelegatedOrganizerAsync(int userId);
        Task<IEnumerable<UserDto>> GetFriendsAsync(int userId);

    }

}