using FootballAPI.Models.Enums;
using FootballAPI.Models;
using FootballAPI.DTOs;
using Microsoft.AspNetCore.Http;

namespace FootballAPI.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<User> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<bool> ReactivateAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> UsernameExistsAsync(string username, int excludeUserId);
        Task<User> AuthenticateAsync(string email, string password);
        Task<bool> ChangePasswordAsync(int userId, string newPassword);
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
        Task<bool> ChangeUsernameAsync(int userId, string newUsername);
        Task<IEnumerable<User>> GetPlayersByOrganiserAsync(int id);
        Task<User?> GetUserByEmail(string email, bool includeDeleted = false, bool tracking = false);

        // Player functionality integrated
        Task<bool> UpdatePlayerRatingAsync(int userId, float ratingChange);
        Task<string> UpdatePlayerProfileImageAsync(int userId, IFormFile imageFile);
        Task<bool> UpdateMultiplePlayerRatingsAsync(List<PlayerRatingUpdateDto> playerRatingUpdates);
        Task AddPlayerOrganiserRelationAsync(PlayerOrganiser relation);
        Task<bool> RemovePlayerOrganiserRelationAsync(int organizerId, int playerId);

        // Organizer delegation functionality
        Task<OrganizerDelegate> CreateDelegationAsync(OrganizerDelegate delegation);
        Task<OrganizerDelegate?> GetActiveDelegationByOrganizerId(int organizerId);
        Task<OrganizerDelegate?> GetActiveDelegationByDelegateId(int delegateId);
        Task<bool> ReclaimDelegationAsync(int delegationId, int originalOrganizerId);
        Task<bool> AreFriends(int userId1, int userId2);
        Task<bool> TransferPlayerOrganiserRelationsAsync(int fromOrganizerId, int toOrganizerId);
        Task<bool> TransferPlayerOrganiserRelationsExcludingAsync(int fromOrganizerId, int toOrganizerId, int excludePlayerId);
        Task<bool> SwitchOrganizerPlayerRelationAsync(int originalOrganizerId, int delegatePlayerId);
        Task<bool> RestoreOrganizerPlayerRelationAsync(int organizerId);
        Task<bool> TransferMatchesAsync(int fromOrganizerId, int toOrganizerId);
        Task<bool> UpdateUserRoleAsync(int userId, UserRole newRole);
    }
}