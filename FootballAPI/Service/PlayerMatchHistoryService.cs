using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository;

namespace FootballAPI.Service
{
    public class PlayerMatchHistoryService : IPlayerMatchHistoryService
    {
        private readonly IPlayerMatchHistoryRepository _playerMatchHistoryRepository;

        public PlayerMatchHistoryService(IPlayerMatchHistoryRepository playerMatchHistoryRepository)
        {
            _playerMatchHistoryRepository = playerMatchHistoryRepository;
        }

        private PlayerMatchHistoryDto MapToDto(PlayerMatchHistory playerMatchHistory)
        {
            return new PlayerMatchHistoryDto
            {
                Id = playerMatchHistory.Id,
                PlayerId = playerMatchHistory.PlayerId,
                Player = playerMatchHistory.Player != null ? new PlayerDto
                {
                    Id = playerMatchHistory.Player.Id,
                    FirstName = playerMatchHistory.Player.FirstName,
                    LastName = playerMatchHistory.Player.LastName,
                    Rating = playerMatchHistory.Player.Rating,
                    IsAvailable = playerMatchHistory.Player.IsAvailable
                } : null,
                TeamId = playerMatchHistory.TeamId,
                Team = playerMatchHistory.Team != null ? new TeamDto
                {
                    Id = playerMatchHistory.Team.Id,
                    Name = playerMatchHistory.Team.Name
                } : null,
                MatchId = playerMatchHistory.MatchId,
                Match = playerMatchHistory.Match != null ? new MatchDto
                {
                    Id = playerMatchHistory.Match.Id,
                    MatchDate = playerMatchHistory.Match.MatchDate,
                    TeamAId = playerMatchHistory.Match.TeamAId,
                    TeamA = playerMatchHistory.Match.TeamA != null ? new TeamDto
                    {
                        Id = playerMatchHistory.Match.TeamA.Id,
                        Name = playerMatchHistory.Match.TeamA.Name
                    } : null,
                    TeamBId = playerMatchHistory.Match.TeamBId,
                    TeamB = playerMatchHistory.Match.TeamB != null ? new TeamDto
                    {
                        Id = playerMatchHistory.Match.TeamB.Id,
                        Name = playerMatchHistory.Match.TeamB.Name
                    } : null,
                    TeamAGoals = playerMatchHistory.Match.TeamAGoals,
                    TeamBGoals = playerMatchHistory.Match.TeamBGoals
                } : null,
                PerformanceRating = playerMatchHistory.PerformanceRating,
                RecordDate = playerMatchHistory.RecordDate
            };
        }

        public async Task<IEnumerable<PlayerMatchHistoryDto>> GetAllPlayerMatchHistoryAsync()
        {
            var playerMatchHistories = await _playerMatchHistoryRepository.GetAllAsync();
            return playerMatchHistories.Select(MapToDto);
        }

        public async Task<PlayerMatchHistoryDto> GetPlayerMatchHistoryByIdAsync(int id)
        {
            var playerMatchHistory = await _playerMatchHistoryRepository.GetByIdAsync(id);
            return playerMatchHistory == null ? null : MapToDto(playerMatchHistory);
        }

        public async Task<IEnumerable<PlayerMatchHistoryDto>> GetPlayerMatchHistoryByPlayerIdAsync(int playerId)
        {
            var playerMatchHistories = await _playerMatchHistoryRepository.GetByPlayerIdAsync(playerId);
            return playerMatchHistories.Select(MapToDto);
        }

        public async Task<IEnumerable<PlayerMatchHistoryDto>> GetPlayerMatchHistoryByTeamIdAsync(int teamId)
        {
            var playerMatchHistories = await _playerMatchHistoryRepository.GetByTeamIdAsync(teamId);
            return playerMatchHistories.Select(MapToDto);
        }

        public async Task<IEnumerable<PlayerMatchHistoryDto>> GetPlayerMatchHistoryByMatchIdAsync(int matchId)
        {
            var playerMatchHistories = await _playerMatchHistoryRepository.GetByMatchIdAsync(matchId);
            return playerMatchHistories.Select(MapToDto);
        }

        public async Task<PlayerMatchHistoryDto> CreatePlayerMatchHistoryAsync(PlayerMatchHistory playerMatchHistory)
        {
            var createdPlayerMatchHistory = await _playerMatchHistoryRepository.CreateAsync(playerMatchHistory);
            return MapToDto(createdPlayerMatchHistory);
        }

        public async Task<PlayerMatchHistoryDto> UpdatePlayerMatchHistoryAsync(int id, PlayerMatchHistory playerMatchHistory)
        {
            var existingPlayerMatchHistory = await _playerMatchHistoryRepository.GetByIdAsync(id);
            if (existingPlayerMatchHistory == null)
                return null;

            existingPlayerMatchHistory.PlayerId = playerMatchHistory.PlayerId;
            existingPlayerMatchHistory.TeamId = playerMatchHistory.TeamId;
            existingPlayerMatchHistory.MatchId = playerMatchHistory.MatchId;
            existingPlayerMatchHistory.PerformanceRating = playerMatchHistory.PerformanceRating;

            var updatedPlayerMatchHistory = await _playerMatchHistoryRepository.UpdateAsync(existingPlayerMatchHistory);
            return MapToDto(updatedPlayerMatchHistory);
        }

        public async Task<bool> DeletePlayerMatchHistoryAsync(int id)
        {
            return await _playerMatchHistoryRepository.DeleteAsync(id);
        }

        public async Task<float> GetAveragePerformanceRatingAsync(int playerId)
        {
            return await _playerMatchHistoryRepository.GetAveragePerformanceRatingAsync(playerId);
        }

        public async Task<IEnumerable<PlayerMatchHistoryDto>> GetTopPerformancesAsync(int count = 10)
        {
            var topPerformances = await _playerMatchHistoryRepository.GetTopPerformancesAsync(count);
            return topPerformances.Select(MapToDto);
        }
    }
}