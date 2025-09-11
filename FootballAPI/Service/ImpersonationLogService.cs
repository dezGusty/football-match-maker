using FootballAPI.Data;
using FootballAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballAPI.Service
{
    public interface IImpersonationLogService
    {
        Task<int> StartImpersonation(int adminId, int impersonatedUserId);
        Task EndImpersonation(int impersonationLogId);
        Task EndAllActiveImpersonationsByAdmin(int adminId);
        Task<List<ImpersonationLog>> GetImpersonationLogsByAdmin(int adminId);
    }

    public class ImpersonationLogService : IImpersonationLogService
    {
        private readonly FootballDbContext _context;
        private readonly ILogger<ImpersonationLogService> _logger;

        public ImpersonationLogService(FootballDbContext context, ILogger<ImpersonationLogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> StartImpersonation(int adminId, int impersonatedUserId)
        {
            try
            {
                // End any active impersonation sessions by this admin
                await EndAllActiveImpersonationsByAdmin(adminId);

                // Create new impersonation log
                var impersonationLog = new ImpersonationLog
                {
                    AdminId = adminId,
                    ImpersonatedUserId = impersonatedUserId,
                    StartTime = DateTime.UtcNow
                };

                _context.ImpersonationLogs.Add(impersonationLog);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Admin {AdminId} started impersonating user {ImpersonatedUserId}. Log ID: {LogId}", 
                    adminId, impersonatedUserId, impersonationLog.Id);
                
                return impersonationLog.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting impersonation log for admin {AdminId} impersonating {UserId}", 
                    adminId, impersonatedUserId);
                throw;
            }
        }

        public async Task EndImpersonation(int impersonationLogId)
        {
            try
            {
                var impersonationLog = await _context.ImpersonationLogs.FindAsync(impersonationLogId);
                if (impersonationLog != null && !impersonationLog.EndTime.HasValue)
                {
                    impersonationLog.EndTime = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Impersonation ended for log ID {LogId}. Admin {AdminId} stopped impersonating user {ImpersonatedUserId}",
                        impersonationLogId, impersonationLog.AdminId, impersonationLog.ImpersonatedUserId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ending impersonation log {LogId}", impersonationLogId);
                throw;
            }
        }

        public async Task EndAllActiveImpersonationsByAdmin(int adminId)
        {
            try
            {
                var activeImpersonations = await _context.ImpersonationLogs
                    .Where(log => log.AdminId == adminId && log.EndTime == null)
                    .ToListAsync();

                foreach (var impersonation in activeImpersonations)
                {
                    impersonation.EndTime = DateTime.UtcNow;
                    _logger.LogInformation("Automatically ending previous impersonation. Admin {AdminId} was impersonating user {ImpersonatedUserId}", 
                        adminId, impersonation.ImpersonatedUserId);
                }

                if (activeImpersonations.Any())
                {
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ending active impersonations for admin {AdminId}", adminId);
                throw;
            }
        }

        public async Task<List<ImpersonationLog>> GetImpersonationLogsByAdmin(int adminId)
        {
            return await _context.ImpersonationLogs
                .Where(log => log.AdminId == adminId)
                .OrderByDescending(log => log.StartTime)
                .ToListAsync();
        }
    }
}
