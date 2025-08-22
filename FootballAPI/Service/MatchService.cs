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
                Location = match.Location,
                Cost = match.Cost,
                OrganiserId = match.OrganiserId
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
                IsPublic = false,
                Status = createMatchDto.Status,
                Location = createMatchDto.Location,
                Cost = createMatchDto.Cost,
                OrganiserId = createMatchDto.OrganiserId
            };

            var createdMatch = await _matchRepository.CreateAsync(match);
            return await GetMatchByIdAsync(createdMatch.Id);
        }

        public async Task<MatchDto> MakeMatchPublicAsync(int matchId)
        {
            var existingMatch = await _matchRepository.GetByIdAsync(matchId);
            if (existingMatch == null)
                return null;

            existingMatch.IsPublic = true;
            var updatedMatch = await _matchRepository.UpdateAsync(existingMatch);
            return MapToDto(updatedMatch);
        }

        public async Task<MatchDto> MakeMatchPrivateAsync(int matchId)
        {
            var existingMatch = await _matchRepository.GetByIdAsync(matchId);
            if (existingMatch == null)
                return null;

            existingMatch.IsPublic = false;
            var updatedMatch = await _matchRepository.UpdateAsync(existingMatch);
            return MapToDto(updatedMatch);
        }

        public async Task<MatchDto> UpdateMatchAsync(int id, UpdateMatchDto updateMatchDto)
        {
            var existingMatch = await _matchRepository.GetByIdAsync(id);
            if (existingMatch == null)
                return null;

            existingMatch.MatchDate = updateMatchDto.MatchDate;
            existingMatch.IsPublic = updateMatchDto.IsPublic;
            existingMatch.Status = updateMatchDto.Status;
            existingMatch.Location = updateMatchDto.Location;
            existingMatch.Cost = updateMatchDto.Cost;

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

        public async Task<IEnumerable<MatchDto>> GetMatchesByOrganiserAsync(int organiserId)
        {
            var matches = await _matchRepository.GetMatchesByOrganiserAsync(organiserId);
            return matches.Select(MapToDto);
        }

        public async Task<IEnumerable<MatchDto>> GetMatchesByLocationAsync(string location)
        {
            var matches = await _matchRepository.GetMatchesByLocationAsync(location);
            return matches.Select(MapToDto);
        }

        public async Task<IEnumerable<MatchDto>> GetMatchesByCostRangeAsync(decimal? minCost, decimal? maxCost)
        {
            var matches = await _matchRepository.GetMatchesByCostRangeAsync(minCost, maxCost);
            return matches.Select(MapToDto);
        }
    }
}