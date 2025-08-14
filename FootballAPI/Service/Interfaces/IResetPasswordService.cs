using FootballAPI.Models;
namespace FootballAPI.Service.Interfaces
{
  public interface IResetPasswordService
  {
    Task<string> GeneratePasswordResetTokenAsync(int userId);
    Task<bool> ValidateAndResetPasswordAsync(string token, string newPassword);
    Task<User?> GetUserByResetTokenAsync(string token);
    Task CleanupExpiredTokensAsync();

  }
}