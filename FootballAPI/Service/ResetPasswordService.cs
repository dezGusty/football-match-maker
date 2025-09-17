using System.Diagnostics.Tracing;
using System.Security.Cryptography;
using System.Text;
using FootballAPI.Data;
using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository;
using FootballAPI.Service.Interfaces;
namespace FootballAPI.Service
{
  public class ResetPasswordService : IResetPasswordService
  {
    private readonly IResetPasswordTokenRepository _tokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ResetPasswordService> _logger;
    private readonly IConfiguration _configuration;

    public ResetPasswordService(
        IResetPasswordTokenRepository tokenRepository,
        IUserRepository userRepository,
        ILogger<ResetPasswordService> logger,
        IConfiguration configuration)
    {
      _tokenRepository = tokenRepository;
      _userRepository = userRepository;
      _logger = logger;
      _configuration = configuration;
    }

    public async Task<string> GeneratePasswordResetTokenAsync(int userId)
    {
      try
      {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
          throw new ArgumentException("User not found");
        }

        var hasActiveToken = await _tokenRepository.HasActiveTokenAsync(userId);
        if (hasActiveToken)
        {
          throw new InvalidOperationException("User already has an active password reset token");
        }

        var tokenBytes = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
          rng.GetBytes(tokenBytes);
        }
        var token = Convert.ToBase64String(tokenBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

        var tokenHash = HashToken(token);

        var expirationHours = _configuration.GetValue<int>("PasswordReset:ExpirationHours", 48);
        var expiresAt = DateTime.UtcNow.AddHours(expirationHours);


        await _tokenRepository.CreateTokenAsync(userId, tokenHash, expiresAt);

        _logger.LogInformation("Password reset token generated for user {UserId}", userId);

        return token;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error generating password reset token for user {UserId}", userId);
        throw;
      }
    }

    public async Task<bool> ValidateAndResetPasswordAsync(string token, string newPassword)
    {
      try
      {
        var tokenHash = HashToken(token);

        var resetToken = await _tokenRepository.GetValidTokenByHashAsync(tokenHash);
        if (resetToken == null)
        {
          _logger.LogWarning("Invalid or expired password reset token attempted");
          return false;
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 11);

        var user = resetToken.User;
        user.Password = hashedPassword;
        await _userRepository.UpdateAsync(user);

        await _tokenRepository.MarkTokenAsUsedAsync(resetToken.Id);
        _logger.LogInformation("Password successfully reset for user {UserId}", user.Id);

        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error resetting password with token");
        return false;
      }
    }
    public async Task<User?> GetUserByResetTokenAsync(string token)
    {
      try
      {
        var tokenHash = HashToken(token);
        var resetToken = await _tokenRepository.GetValidTokenByHashAsync(tokenHash);
        return resetToken?.User;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error getting user by reset token");
        return null;
      }
    }

    private string HashToken(string token)
    {
      using (var sha256 = SHA256.Create())
      {
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashedBytes);
      }
    }

    public async Task<bool> HasActiveTokenAsync(int userId)
    {
      try
      {
        return await _tokenRepository.HasActiveTokenAsync(userId);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error checking for active tokens for user {UserId}", userId);
        return false;
      }
    }
  }
}