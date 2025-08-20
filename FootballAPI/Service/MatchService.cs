using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository;

namespace FootballAPI.Service
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;

        public MatchService(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        private MatchDto MapToDto(Match match)
        {
            return new MatchDto
            {
                Id = match.Id,
                MatchDate = match.MatchDate,
                IsPublic = match.IsPublic,
                Status = match.Status,
                PlayerHistory = match.PlayerHistory?.Select(ph => new PlayerMatchHistoryDto
                {
                    Id = ph.Id,
                    PlayerId = ph.PlayerId,
                    Player = ph.Player != null ? new PlayerDto
                    {
                        Id = ph.Player.Id,
                        FirstName = ph.Player.FirstName,
                        LastName = ph.Player.LastName,
                        Rating = ph.Player.Rating,
                        IsAvailable = ph.Player.IsAvailable
                    } : null,
                    TeamId = ph.TeamId,
                    PerformanceRating = ph.PerformanceRating,
                    RecordDate = ph.RecordDate
                }).ToList() ?? new List<PlayerMatchHistoryDto>()
            };
        }

        public async Task<IEnumerable<MatchDto>> GetAllMatchesAsync()
        {
            var matches = await _matchRepository.GetAllAsync();
            return matches.Select(MapToDto);
        }

        public async Task<MatchDto> GetMatchByIdAsync(int id)
        {
            var match = await _matchRepository.GetByIdAsync(id);
            return match == null ? null : MapToDto(match);
        }

        public async Task<IEnumerable<MatchDto>> GetMatchesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var matches = await _matchRepository.GetMatchesByDateRangeAsync(startDate, endDate);
            return matches.Select(MapToDto);
        }

        public async Task<IEnumerable<MatchDto>> GetPublicMatchesAsync()
        {
            var matches = await _matchRepository.GetPublicMatchesAsync();
            return matches.Select(MapToDto);
        }

        public async Task<IEnumerable<MatchDto>> GetMatchesByStatusAsync(Status status)
        {
            var matches = await _matchRepository.GetMatchesByStatusAsync(status);
            return matches.Select(MapToDto);
        }

        public async Task<MatchDto> CreateMatchAsync(CreateMatchDto createMatchDto)
        {
            var match = new Match
            {
                MatchDate = createMatchDto.MatchDate,
                IsPublic = createMatchDto.IsPublic,
                Status = createMatchDto.Status
            };

            var createdMatch = await _matchRepository.CreateAsync(match);
            return await GetMatchByIdAsync(createdMatch.Id);
        }

        public async Task<MatchDto> UpdateMatchAsync(int id, UpdateMatchDto updateMatchDto)
        {
            var existingMatch = await _matchRepository.GetByIdAsync(id);
            if (existingMatch == null)
                return null;

            existingMatch.MatchDate = updateMatchDto.MatchDate;
            existingMatch.IsPublic = updateMatchDto.IsPublic;
            existingMatch.Status = updateMatchDto.Status;

            var updatedMatch = await _matchRepository.UpdateAsync(existingMatch);
            return MapToDto(updatedMatch);
        }

        public async Task<bool> DeleteMatchAsync(int id)
        {
            return await _matchRepository.DeleteAsync(id);
        }
        public async Task<IEnumerable<MatchDto>> GetFutureMatchesAsync()
        {
            var futureMatches = await _matchRepository.GetFutureMatchesAsync();
            return futureMatches.Select(MapToDto);

        }
        public async Task<IEnumerable<MatchDto>> GetPastMatchesAsync()
        {
            var pastMatches = await _matchRepository.GetPastMatchesAsync();
            return pastMatches.Select(MapToDto);
        }
    }
}