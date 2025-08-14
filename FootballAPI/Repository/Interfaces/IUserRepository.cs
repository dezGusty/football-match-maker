using FootballAPI.Models;
using FootballAPI.DTOs;

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
        Task<bool> ExistsAsync(int id);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> UsernameExistsAsync(string username, int excludeUserId);
        Task<User> AuthenticateAsync(string email, string password);
        Task<bool> ChangePasswordAsync(int userId, string newPassword);
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
        Task<bool> ChangeUsernameAsync(int userId, string newUsername);
        Task<IEnumerable<Player>> GetPlayersByOrganiserAsync(int id);
        Task<User?> GetUserByEmail(string email, bool includeDeleted = false, bool tracking = false);
    }
}