using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository;
using FootballAPI.Repository.Interfaces;
using FootballAPI.Service.Interfaces;

namespace FootballAPI.Service
{
    public class MatchHistoryService : IMatchHistoryService
    {
        private readonly IMatchHistoryRepository _matchHistoryRepository;
        private readonly IPlayerMatchStatsRepository _playerMatchStatsRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IMatchPlayerRepository _matchPlayerRepository;

        public MatchHistoryService(
            IMatchHistoryRepository matchHistoryRepository,
            IPlayerMatchStatsRepository playerMatchStatsRepository,
            IMatchRepository matchRepository,
            IMatchPlayerRepository matchPlayerRepository)
        {
            _matchHistoryRepository = matchHistoryRepository;
            _playerMatchStatsRepository = playerMatchStatsRepository;
            _matchRepository = matchRepository;
            _matchPlayerRepository = matchPlayerRepository;
        }

        public async Task<IEnumerable<MatchHistoryDto>> GetAllMatchHistoriesAsync()
        {
            var matchHistories = await _matchHistoryRepository.GetAllMatchHistoriesAsync();
            return matchHistories.Select(MapToMatchHistoryDto);
        }

        public async Task<MatchHistoryDto?> GetMatchHistoryByIdAsync(int id)
        {
            var matchHistory = await _matchHistoryRepository.GetMatchHistoryByIdAsync(id);
            return matchHistory != null ? MapToMatchHistoryDto(matchHistory) : null;
        }

        public async Task<MatchHistoryDto?> GetMatchHistoryByMatchIdAsync(int matchId)
        {
            var matchHistory = await _matchHistoryRepository.GetMatchHistoryByMatchIdAsync(matchId);
            return matchHistory != null ? MapToMatchHistoryDto(matchHistory) : null;
        }

        public async Task<IEnumerable<MatchHistorySummaryDto>> GetPlayerMatchHistoriesAsync(int playerId)
        {
            var matchHistories = await _matchHistoryRepository.GetPlayerMatchHistoriesAsync(playerId);
            return matchHistories.Select(MapToMatchHistorySummaryDto);
        }

        public async Task<IEnumerable<MatchHistorySummaryDto>> GetOrganiserMatchHistoriesAsync(int organiserId)
        {
            var matchHistories = await _matchHistoryRepository.GetOrganiserMatchHistoriesAsync(organiserId);
            return matchHistories.Select(MapToMatchHistorySummaryDto);
        }

        public async Task<MatchHistoryDto> CreateMatchHistoryAsync(CreateMatchHistoryDto createMatchHistoryDto, int organiserId)
        {
            var match = await _matchRepository.GetMatchByIdAsync(createMatchHistoryDto.MatchId);
            if (match == null)
                throw new KeyNotFoundException("Match not found");

            if (match.OrganiserId != organiserId)
                throw new UnauthorizedAccessException("Only the organiser can create match history");

            if (match.Status != MatchStatus.InProgress && match.Status != MatchStatus.Scheduled)
                throw new InvalidOperationException("Can only create history for matches that are in progress or scheduled");

            var existingHistory = await _matchHistoryRepository.GetMatchHistoryByMatchIdAsync(createMatchHistoryDto.MatchId);
            if (existingHistory != null)
                throw new InvalidOperationException("Match history already exists for this match");

            var matchHistory = new MatchHistory
            {
                MatchId = createMatchHistoryDto.MatchId,
                Team1Score = createMatchHistoryDto.Team1Score,
                Team2Score = createMatchHistoryDto.Team2Score,
                Duration = createMatchHistoryDto.Duration,
                CompletedAt = DateTime.Now
            };

            var createdMatchHistory = await _matchHistoryRepository.CreateMatchHistoryAsync(matchHistory);

            foreach (var playerStatDto in createMatchHistoryDto.PlayerStats)
            {
                var playerStat = new PlayerMatchStats
                {
                    MatchHistoryId = createdMatchHistory.Id,
                    PlayerId = playerStatDto.PlayerId,
                    Goals = playerStatDto.Goals,
                    Assists = playerStatDto.Assists,
                    TeamNumber = playerStatDto.TeamNumber,
                    Rating = playerStatDto.Rating
                };

                await _playerMatchStatsRepository.CreatePlayerMatchStatAsync(playerStat);
            }

            match.Status = MatchStatus.Completed;
            await _matchRepository.UpdateMatchAsync(match);

            var updatedMatchHistory = await _matchHistoryRepository.GetMatchHistoryByIdAsync(createdMatchHistory.Id);
            return MapToMatchHistoryDto(updatedMatchHistory!);
        }

        public async Task<MatchHistoryDto> UpdateMatchHistoryAsync(int id, UpdateMatchHistoryDto updateMatchHistoryDto, int organiserId)
        {
            var matchHistory = await _matchHistoryRepository.GetMatchHistoryByIdAsync(id);
            if (matchHistory == null)
                throw new KeyNotFoundException("Match history not found");

            if (matchHistory.Match.OrganiserId != organiserId)
                throw new UnauthorizedAccessException("Only the organiser can update match history");

            matchHistory.Team1Score = updateMatchHistoryDto.Team1Score;
            matchHistory.Team2Score = updateMatchHistoryDto.Team2Score;
            matchHistory.Duration = updateMatchHistoryDto.Duration;

            var updatedMatchHistory = await _matchHistoryRepository.UpdateMatchHistoryAsync(matchHistory);
            return MapToMatchHistoryDto(updatedMatchHistory);
        }

        public async Task<bool> DeleteMatchHistoryAsync(int id, int organiserId)
        {
            var matchHistory = await _matchHistoryRepository.GetMatchHistoryByIdAsync(id);
            if (matchHistory == null)
                return false;

            if (matchHistory.Match.OrganiserId != organiserId)
                throw new UnauthorizedAccessException("Only the organiser can delete match history");

            var match = matchHistory.Match;
            match.Status = MatchStatus.Scheduled;
            await _matchRepository.UpdateMatchAsync(match);

            return await _matchHistoryRepository.DeleteMatchHistoryAsync(id);
        }

        public async Task<PlayerStatsAggregateDto> GetPlayerStatsAggregateAsync(int playerId)
        {
            var playerStats = await _playerMatchStatsRepository.GetPlayerStatsAsync(playerId);
            var player = playerStats.FirstOrDefault()?.Player;

            if (player == null)
                throw new KeyNotFoundException("Player not found or has no match history");

            var totalMatches = playerStats.Count();
            var totalGoals = playerStats.Sum(ps => ps.Goals);
            var totalAssists = playerStats.Sum(ps => ps.Assists);
            var averageRating = await _playerMatchStatsRepository.GetPlayerAverageRatingAsync(playerId);

            var wins = 0;
            var losses = 0;
            var draws = 0;

            foreach (var stat in playerStats)
            {
                var team1Score = stat.MatchHistory.Team1Score;
                var team2Score = stat.MatchHistory.Team2Score;
                var playerTeam = stat.TeamNumber;

                if (team1Score == team2Score)
                {
                    draws++;
                }
                else if ((playerTeam == 1 && team1Score > team2Score) || (playerTeam == 2 && team2Score > team1Score))
                {
                    wins++;
                }
                else
                {
                    losses++;
                }
            }

            return new PlayerStatsAggregateDto
            {
                PlayerId = playerId,
                PlayerFirstName = player.FirstName,
                PlayerLastName = player.LastName,
                PlayerUsername = player.User?.Username ?? string.Empty,
                TotalMatches = totalMatches,
                TotalGoals = totalGoals,
                TotalAssists = totalAssists,
                AverageRating = averageRating,
                Wins = wins,
                Losses = losses,
                Draws = draws
            };
        }

        private MatchHistoryDto MapToMatchHistoryDto(MatchHistory matchHistory)
        {
            var result = "Draw";
            if (matchHistory.Team1Score > matchHistory.Team2Score)
                result = "Team 1 Win";
            else if (matchHistory.Team2Score > matchHistory.Team1Score)
                result = "Team 2 Win";

            return new MatchHistoryDto
            {
                Id = matchHistory.Id,
                MatchId = matchHistory.MatchId,
                MatchTitle = matchHistory.Match?.Title ?? string.Empty,
                OrganiserUsername = matchHistory.Match?.Organiser?.Username ?? string.Empty,
                Location = matchHistory.Match?.Location ?? string.Empty,
                MatchDateTime = matchHistory.Match?.DateTime ?? DateTime.MinValue,
                Team1Score = matchHistory.Team1Score,
                Team2Score = matchHistory.Team2Score,
                Duration = matchHistory.Duration,
                CompletedAt = matchHistory.CompletedAt,
                Result = result,
                PlayerStats = matchHistory.PlayerStats?.Select(MapToPlayerMatchStatsDto).ToList() ?? new List<PlayerMatchStatsDto>()
            };
        }

        private MatchHistorySummaryDto MapToMatchHistorySummaryDto(MatchHistory matchHistory)
        {
            var result = "Draw";
            if (matchHistory.Team1Score > matchHistory.Team2Score)
                result = "Team 1 Win";
            else if (matchHistory.Team2Score > matchHistory.Team1Score)
                result = "Team 2 Win";

            return new MatchHistorySummaryDto
            {
                Id = matchHistory.Id,
                MatchTitle = matchHistory.Match?.Title ?? string.Empty,
                OrganiserUsername = matchHistory.Match?.Organiser?.Username ?? string.Empty,
                Location = matchHistory.Match?.Location ?? string.Empty,
                MatchDateTime = matchHistory.Match?.DateTime ?? DateTime.MinValue,
                Team1Score = matchHistory.Team1Score,
                Team2Score = matchHistory.Team2Score,
                Result = result,
                CompletedAt = matchHistory.CompletedAt
            };
        }

        private PlayerMatchStatsDto MapToPlayerMatchStatsDto(PlayerMatchStats playerMatchStats)
        {
            return new PlayerMatchStatsDto
            {
                Id = playerMatchStats.Id,
                MatchHistoryId = playerMatchStats.MatchHistoryId,
                PlayerId = playerMatchStats.PlayerId,
                PlayerFirstName = playerMatchStats.Player?.FirstName ?? string.Empty,
                PlayerLastName = playerMatchStats.Player?.LastName ?? string.Empty,
                PlayerUsername = playerMatchStats.Player?.User?.Username ?? string.Empty,
                Goals = playerMatchStats.Goals,
                Assists = playerMatchStats.Assists,
                TeamNumber = playerMatchStats.TeamNumber,
                Rating = playerMatchStats.Rating,
                MatchTitle = playerMatchStats.MatchHistory?.Match?.Title ?? string.Empty,
                MatchDate = playerMatchStats.MatchHistory?.CompletedAt ?? DateTime.MinValue
            };
        }
    }
}