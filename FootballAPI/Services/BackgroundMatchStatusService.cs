using Microsoft.EntityFrameworkCore;
using FootballAPI.Data;
using FootballAPI.Models.Enums;
using FootballAPI.Service;

namespace FootballAPI.Services
{
    public class BackgroundMatchStatusService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BackgroundMatchStatusService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromMinutes(1); // Run every 1 minute

        public BackgroundMatchStatusService(IServiceProvider serviceProvider, ILogger<BackgroundMatchStatusService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_period);

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await UpdateMatchStatuses();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred updating match statuses");
                }
            }
        }

        private async Task UpdateMatchStatuses()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FootballDbContext>();
            var matchService = scope.ServiceProvider.GetRequiredService<IMatchService>();

            var openMatches = await context.Matches
                .Where(m => m.Status == Status.Open && m.MatchDate < DateTime.Now)
                .ToListAsync();

            foreach (var match in openMatches)
            {
                try
                {
                    var matchDetails = await matchService.GetMatchDetailsAsync(match.Id);

                    if (matchDetails.TotalPlayers >= 10)
                    {
                        // Automatically finalize matches with 10+ players that have passed their date
                        match.Status = Status.Finalized;
                        _logger.LogInformation($"Auto-finalized match {match.Id} with {matchDetails.TotalPlayers} players");
                    }
                    else
                    {
                        // Automatically cancel matches with less than 10 players that have passed their date
                        match.Status = Status.Cancelled;

                        _logger.LogInformation($"Auto-cancelled match {match.Id} with {matchDetails.TotalPlayers} players");
                    }

                    context.Matches.Update(match);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating status for match {match.Id}");
                }
            }

            await context.SaveChangesAsync();
        }
    }
}