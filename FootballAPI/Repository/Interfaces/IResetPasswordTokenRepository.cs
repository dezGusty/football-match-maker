using Microsoft.EntityFrameworkCore;
using FootballAPI.Data;
using FootballAPI.Models;

namespace FootballAPI.Repository
{ 
    public interface IResetPasswordTokenRepository
    {
        Task<ResetPasswordToken> CreateTokenAsync(int userId, string tokenHash, DateTime expiresAt);
        Task<ResetPasswordToken?> GetValidTokenByHashAsync(string tokenHash);
        Task MarkTokenAsUsedAsync(int tokenId);
        Task InvalidateAllUserTokensAsync(int userId);
        Task CleanupExpiredTokensAsync();
        Task<bool> HasActiveTokenAsync(int userId);
    }
}
