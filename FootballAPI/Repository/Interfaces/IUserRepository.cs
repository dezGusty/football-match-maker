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
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> UsernameExistsAsync(string username, int excludeUserId);
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
        Task<IEnumerable<User>> GetPlayersByOrganiserAsync(int id);
        Task<User?> GetUserByEmail(string email, bool includeDeleted = false, bool tracking = false);

        Task<bool> UpdatePlayerRatingAsync(int userId, float ratingChange);
        Task<bool> UpdateMultiplePlayerRatingsAsync(List<PlayerRatingUpdateDto> playerRatingUpdates);
        Task AddPlayerOrganiserRelationAsync(int organizerId, int playerId);
        Task<OrganizerDelegate> CreateDelegationAsync(OrganizerDelegate delegation);
        Task<OrganizerDelegate?> GetActiveDelegationByOrganizerId(int organizerId);
        Task<OrganizerDelegate?> GetActiveDelegationByDelegateId(int delegateId);
        Task<bool> ReclaimDelegationAsync(int delegationId, int originalOrganizerId);
        Task<bool> AreFriends(int userId1, int userId2);
        Task<bool> TransferPlayerOrganiserRelationsExcludingAsync(int fromOrganizerId, int toOrganizerId, int excludePlayerId);
        Task<bool> SwitchOrganizerPlayerRelationAsync(int originalOrganizerId, int delegatePlayerId);
        Task<bool> TransferMatchesAsync(int fromOrganizerId, int toOrganizerId);
        Task<bool> UpdateUserRoleAsync(int userId, UserRole newRole);
        Task<FriendRequest?> GetFriendRequestRelationAsync(int userId, int friendId);
        Task DeleteFriendRequestRelationAsync(FriendRequest relation);


    }
}