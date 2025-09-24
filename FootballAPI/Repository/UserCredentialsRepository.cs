using Microsoft.EntityFrameworkCore;
using FootballAPI.Data;
using FootballAPI.Models;
using FootballAPI.Repository.Interfaces;

namespace FootballAPI.Repository
{
  public class UserCredentialsRepository : IUserCredentialsRepository
  {
    private readonly FootballDbContext _context;

    public UserCredentialsRepository(FootballDbContext context)
    {
      _context = context;
    }

    public async Task<UserCredentials?> GetByUserIdAsync(int userId)
    {
      return await _context.UserCredentials
          .Include(uc => uc.User)
          .FirstOrDefaultAsync(uc => uc.UserId == userId);
    }

    public async Task<UserCredentials?> GetByEmailAsync(string email)
    {
      return await _context.UserCredentials
          .Include(uc => uc.User)
          .FirstOrDefaultAsync(uc => uc.Email == email);
    }

    public async Task<UserCredentials> CreateAsync(UserCredentials credentials)
    {
      _context.UserCredentials.Add(credentials);
      await _context.SaveChangesAsync();
      return credentials;
    }

    public async Task<UserCredentials> UpdateAsync(UserCredentials credentials)
    {
      _context.Entry(credentials).State = EntityState.Modified;
      await _context.SaveChangesAsync();
      return credentials;
    }

    public async Task<bool> DeleteAsync(int id)
    {
      var credentials = await _context.UserCredentials.FindAsync(id);
      if (credentials == null)
        return false;

      _context.UserCredentials.Remove(credentials);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
      return await _context.UserCredentials
          .AnyAsync(uc => uc.Email.ToLower() == email.ToLower());
    }

    public async Task<bool> EmailExistsAsync(string email, int excludeUserId)
    {
      return await _context.UserCredentials
          .AnyAsync(uc => uc.Email.ToLower() == email.ToLower() && uc.UserId != excludeUserId);
    }
  }
}