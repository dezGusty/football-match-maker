using Microsoft.EntityFrameworkCore;
using FootballAPI.Data;
using FootballAPI.Models;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using static FootballAPI.Repository.UserRepository;

namespace FootballAPI.Repository
{
    public class ResetPasswordTokenRepository : IResetPasswordTokenRepository
    {
        private readonly FootballDbContext _context;

        public ResetPasswordTokenRepository(FootballDbContext context)
        {
            _context = context;
        }

        public async Task<ResetPasswordToken> CreateTokenAsync(int userId, string tokenHash, DateTime expiresAt)
        {
            await InvalidateAllUserTokensAsync(userId);

            var token = new ResetPasswordToken
            {
                UserId = userId,
                TokenHash = tokenHash,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow

            };

            _context.ResetPasswordTokens.Add(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<ResetPasswordToken?> GetValidTokenByHashAsync(string tokenHash)
        {
            Console.WriteLine($"Caut token cu hash: {tokenHash}");
            return await _context.ResetPasswordTokens
                .Include(t => t.User)
                .ThenInclude(u => u.Credentials)
                .FirstOrDefaultAsync(t =>
                    t.TokenHash == tokenHash &&
                    t.UsedAt == null &&
                    t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task MarkTokenAsUsedAsync(int tokenId)
        {
            var token = await _context.ResetPasswordTokens.FindAsync(tokenId);
            if (token != null)
            {
                token.MarkAsUsed();
                await _context.SaveChangesAsync();
            }
        }

        public async Task InvalidateAllUserTokensAsync(int userId)
        {
            var tokens = await _context.ResetPasswordTokens
                .Where(t => t.UserId == userId && t.UsedAt == null)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.MarkAsUsed();
            }

            if (tokens.Any())
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasActiveTokenAsync(int userId)
        {
            return await _context.ResetPasswordTokens
                .AnyAsync(t =>
                     t.UserId == userId &&
                     t.UsedAt == null &&
                     t.ExpiresAt > DateTime.UtcNow);
        }
    }
}