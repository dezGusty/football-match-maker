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
                TeamAId = match.TeamAId,
                TeamA = match.TeamA != null ? new TeamDto 
                { 
                    Id = match.TeamA.Id, 
                    Name = match.TeamA.Name,
                    CurrentPlayers = match.TeamA.CurrentPlayers?.Select(p => new PlayerDto
                    {
                        Id = p.Id,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Rating = p.Rating,
                        IsAvailable = p.IsAvailable,
                        CurrentTeamId = p.CurrentTeamId
                    }).ToList()
                } : null,
                TeamBId = match.TeamBId,
                TeamB = match.TeamB != null ? new TeamDto 
                { 
                    Id = match.TeamB.Id, 
                    Name = match.TeamB.Name,
                    CurrentPlayers = match.TeamB.CurrentPlayers?.Select(p => new PlayerDto
                    {
                        Id = p.Id,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Rating = p.Rating,
                        IsAvailable = p.IsAvailable,
                        CurrentTeamId = p.CurrentTeamId
                    }).ToList()
                } : null,
                TeamAGoals = match.TeamAGoals,
                TeamBGoals = match.TeamBGoals,
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
                        IsAvailable = ph.Player.IsAvailable,
                        CurrentTeamId = ph.Player.CurrentTeamId
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

        public async Task<IEnumerable<MatchDto>> GetMatchesByTeamIdAsync(int teamId)
        {
            var matches = await _matchRepository.GetMatchesByTeamIdAsync(teamId);
            return matches.Select(MapToDto);
        }

        public async Task<MatchDto> CreateMatchAsync(CreateMatchDto createMatchDto)
        {
            var match = new Match
            {
                MatchDate = createMatchDto.MatchDate,
                TeamAId = createMatchDto.TeamAId,
                TeamBId = createMatchDto.TeamBId,
                TeamAGoals = 0,
                TeamBGoals = 0
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
            existingMatch.TeamAId = updateMatchDto.TeamAId;
            existingMatch.TeamBId = updateMatchDto.TeamBId;
            existingMatch.TeamAGoals = updateMatchDto.TeamAGoals;
            existingMatch.TeamBGoals = updateMatchDto.TeamBGoals;

            var updatedMatch = await _matchRepository.UpdateAsync(existingMatch);
            return MapToDto(updatedMatch);
        }

        public async Task<bool> DeleteMatchAsync(int id)
        {
            return await _matchRepository.DeleteAsync(id);
        }
    }
} 