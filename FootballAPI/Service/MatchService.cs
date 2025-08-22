using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository;
using FootballAPI.Repository.Interfaces;
using FootballAPI.Service.Interfaces;

namespace FootballAPI.Service
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMatchPlayerRepository _matchPlayerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPlayerRepository _playerRepository;

        public MatchService(
            IMatchRepository matchRepository,
            IMatchPlayerRepository matchPlayerRepository,
            IUserRepository userRepository,
            IPlayerRepository playerRepository)
        {
            _matchRepository = matchRepository;
            _matchPlayerRepository = matchPlayerRepository;
            _userRepository = userRepository;
            _playerRepository = playerRepository;
        }

        public async Task<IEnumerable<MatchDto>> GetAllMatchesAsync()
        {
            var matches = await _matchRepository.GetAllMatchesAsync();
            return matches.Select(MapToMatchDto);
        }

        public async Task<IEnumerable<PublicMatchDto>> GetPublicMatchesAsync()
        {
            var matches = await _matchRepository.GetPublicMatchesAsync();
            return matches.Select(MapToPublicMatchDto);
        }

        public async Task<IEnumerable<MatchDto>> GetMatchesByOrganiserAsync(int organiserId)
        {
            var matches = await _matchRepository.GetMatchesByOrganiserAsync(organiserId);
            return matches.Select(MapToMatchDto);
        }

        public async Task<MatchDto?> GetMatchByIdAsync(int id)
        {
            var match = await _matchRepository.GetMatchByIdAsync(id);
            return match != null ? MapToMatchDto(match) : null;
        }

        public async Task<MatchDto> CreateMatchAsync(CreateMatchDto createMatchDto, int organiserId)
        {
            var organiser = await _userRepository.GetByIdAsync(organiserId);
            if (organiser == null || organiser.Role != UserRole.ORGANISER)
                throw new UnauthorizedAccessException("Only organisers can create matches");

            var match = new Match
            {
                OrganiserId = organiserId,
                Title = createMatchDto.Title,
                Description = createMatchDto.Description,
                Location = createMatchDto.Location,
                DateTime = createMatchDto.DateTime,
                IsPublic = createMatchDto.IsPublic,
                MaxPlayers = createMatchDto.MaxPlayers,
                Status = MatchStatus.Scheduled,
                CreatedAt = DateTime.Now
            };

            var createdMatch = await _matchRepository.CreateMatchAsync(match);
            return MapToMatchDto(createdMatch);
        }

        public async Task<MatchDto> UpdateMatchAsync(int id, UpdateMatchDto updateMatchDto, int organiserId)
        {
            var match = await _matchRepository.GetMatchByIdAsync(id);
            if (match == null)
                throw new KeyNotFoundException("Match not found");

            if (match.OrganiserId != organiserId)
                throw new UnauthorizedAccessException("Only the organiser can update this match");

            if (match.Status != MatchStatus.Scheduled)
                throw new InvalidOperationException("Cannot update a match that is not scheduled");

            match.Title = updateMatchDto.Title;
            match.Description = updateMatchDto.Description;
            match.Location = updateMatchDto.Location;
            match.DateTime = updateMatchDto.DateTime;
            match.IsPublic = updateMatchDto.IsPublic;
            match.MaxPlayers = updateMatchDto.MaxPlayers;

            if (Enum.TryParse<MatchStatus>(updateMatchDto.Status, true, out var status))
                match.Status = status;

            var updatedMatch = await _matchRepository.UpdateMatchAsync(match);
            return MapToMatchDto(updatedMatch);
        }

        public async Task<bool> DeleteMatchAsync(int id, int organiserId)
        {
            var match = await _matchRepository.GetMatchByIdAsync(id);
            if (match == null)
                return false;

            if (match.OrganiserId != organiserId)
                throw new UnauthorizedAccessException("Only the organiser can delete this match");

            if (match.Status == MatchStatus.InProgress || match.Status == MatchStatus.Completed)
                throw new InvalidOperationException("Cannot delete a match that is in progress or completed");

            return await _matchRepository.DeleteMatchAsync(id);
        }

        public async Task<MatchPlayerDto> JoinMatchAsync(JoinMatchDto joinMatchDto, int playerId)
        {
            var match = await _matchRepository.GetMatchByIdAsync(joinMatchDto.MatchId);
            if (match == null)
                throw new KeyNotFoundException("Match not found");

            if (match.Status != MatchStatus.Scheduled)
                throw new InvalidOperationException("Cannot join a match that is not scheduled");

            if (!match.IsPublic)
            {
                var playerOrganiser = await _playerRepository.GetPlayerOrganiserAsync(playerId, match.OrganiserId);
                if (playerOrganiser == null)
                    throw new UnauthorizedAccessException("You must be friends with the organiser to join this private match");
            }

            var isPlayerInMatch = await _matchPlayerRepository.IsPlayerInMatchAsync(joinMatchDto.MatchId, playerId);
            if (isPlayerInMatch)
                throw new InvalidOperationException("Player is already in this match");

            var isMatchFull = await _matchRepository.IsMatchFullAsync(joinMatchDto.MatchId);
            if (isMatchFull)
                throw new InvalidOperationException("Match is full");

            var team1Count = await _matchPlayerRepository.GetTeamPlayerCountAsync(joinMatchDto.MatchId, 1);
            var team2Count = await _matchPlayerRepository.GetTeamPlayerCountAsync(joinMatchDto.MatchId, 2);
            var teamNumber = team1Count <= team2Count ? 1 : 2;

            var matchPlayer = new MatchPlayer
            {
                MatchId = joinMatchDto.MatchId,
                PlayerId = playerId,
                TeamNumber = teamNumber,
                JoinedAt = DateTime.Now,
                IsConfirmed = true
            };

            var createdMatchPlayer = await _matchPlayerRepository.AddPlayerToMatchAsync(matchPlayer);
            return MapToMatchPlayerDto(createdMatchPlayer);
        }

        public async Task<bool> LeaveMatchAsync(int matchId, int playerId)
        {
            var match = await _matchRepository.GetMatchByIdAsync(matchId);
            if (match == null)
                return false;

            if (match.Status != MatchStatus.Scheduled)
                throw new InvalidOperationException("Cannot leave a match that is not scheduled");

            return await _matchPlayerRepository.RemovePlayerFromMatchAsync(matchId, playerId);
        }

        public async Task<bool> UpdatePlayerTeamAsync(UpdatePlayerTeamDto updatePlayerTeamDto, int organiserId)
        {
            var match = await _matchRepository.GetMatchByIdAsync(updatePlayerTeamDto.MatchId);
            if (match == null)
                throw new KeyNotFoundException("Match not found");

            if (match.OrganiserId != organiserId)
                throw new UnauthorizedAccessException("Only the organiser can change player teams");

            if (match.Status != MatchStatus.Scheduled)
                throw new InvalidOperationException("Cannot change teams for a match that is not scheduled");

            return await _matchPlayerRepository.UpdatePlayerTeamAsync(
                updatePlayerTeamDto.MatchId,
                updatePlayerTeamDto.PlayerId,
                updatePlayerTeamDto.TeamNumber);
        }

        public async Task<IEnumerable<TeamDto>> GetMatchTeamsAsync(int matchId)
        {
            var team1Players = await _matchPlayerRepository.GetTeamPlayersAsync(matchId, 1);
            var team2Players = await _matchPlayerRepository.GetTeamPlayersAsync(matchId, 2);

            var teams = new List<TeamDto>
            {
                new TeamDto
                {
                    TeamNumber = 1,
                    Players = team1Players.Select(MapToMatchPlayerDto).ToList(),
                    PlayerCount = team1Players.Count(),
                    AverageRating = team1Players.Any() ? team1Players.Average(p => p.Player.Rating) : 0
                },
                new TeamDto
                {
                    TeamNumber = 2,
                    Players = team2Players.Select(MapToMatchPlayerDto).ToList(),
                    PlayerCount = team2Players.Count(),
                    AverageRating = team2Players.Any() ? team2Players.Average(p => p.Player.Rating) : 0
                }
            };

            return teams;
        }

        private MatchDto MapToMatchDto(Match match)
        {
            return new MatchDto
            {
                Id = match.Id,
                OrganiserId = match.OrganiserId,
                OrganiserUsername = match.Organiser?.Username ?? string.Empty,
                Title = match.Title,
                Description = match.Description,
                Location = match.Location,
                DateTime = match.DateTime,
                IsPublic = match.IsPublic,
                Status = match.Status.ToString(),
                MaxPlayers = match.MaxPlayers,
                CurrentPlayerCount = match.MatchPlayers?.Count ?? 0,
                CreatedAt = match.CreatedAt,
                Players = match.MatchPlayers?.Select(MapToMatchPlayerDto).ToList() ?? new List<MatchPlayerDto>()
            };
        }

        private PublicMatchDto MapToPublicMatchDto(Match match)
        {
            var currentPlayerCount = match.MatchPlayers?.Count ?? 0;
            return new PublicMatchDto
            {
                Id = match.Id,
                OrganiserUsername = match.Organiser?.Username ?? string.Empty,
                Title = match.Title,
                Description = match.Description,
                Location = match.Location,
                DateTime = match.DateTime,
                MaxPlayers = match.MaxPlayers,
                CurrentPlayerCount = currentPlayerCount,
                AvailableSpots = match.MaxPlayers - currentPlayerCount
            };
        }

        private MatchPlayerDto MapToMatchPlayerDto(MatchPlayer matchPlayer)
        {
            return new MatchPlayerDto
            {
                MatchId = matchPlayer.MatchId,
                MatchTitle = matchPlayer.Match?.Title ?? string.Empty,
                PlayerId = matchPlayer.PlayerId,
                PlayerFirstName = matchPlayer.Player?.FirstName ?? string.Empty,
                PlayerLastName = matchPlayer.Player?.LastName ?? string.Empty,
                PlayerUsername = matchPlayer.Player?.User?.Username ?? string.Empty,
                PlayerRating = matchPlayer.Player?.Rating ?? 0,
                TeamNumber = matchPlayer.TeamNumber,
                JoinedAt = matchPlayer.JoinedAt,
                IsConfirmed = matchPlayer.IsConfirmed
            };
        }
    }
}