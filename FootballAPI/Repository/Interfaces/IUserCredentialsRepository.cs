using FootballAPI.Models;

namespace FootballAPI.Repository.Interfaces
{
  public interface IUserCredentialsRepository
  {
    Task<UserCredentials?> GetByUserIdAsync(int userId);
    Task<UserCredentials?> GetByEmailAsync(string email);
    Task<UserCredentials> CreateAsync(UserCredentials credentials);
    Task<UserCredentials> UpdateAsync(UserCredentials credentials);
    Task<bool> DeleteAsync(int id);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> EmailExistsAsync(string email, int excludeUserId);
  }
}