using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Models.Enums;
using FootballAPI.Repository;
using FootballAPI.Data;

namespace FootballAPI.Service
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamService _teamService;
        private readonly IMatchTeamsService _matchTeamsService;
        private readonly ITeamPlayersService _teamPlayersService;
        private readonly IUserService _userService;
        private readonly IPlayerService _playerService;
        private readonly FootballDbContext _context;

        public MatchService(IMatchRepository matchRepository, ITeamService teamService,
                           IMatchTeamsService matchTeamsService, ITeamPlayersService teamPlayersService,
                           IUserService userService, IPlayerService playerService, FootballDbContext context)
        {
            _matchRepository = matchRepository;
            _teamService = teamService;
            _matchTeamsService = matchTeamsService;
            _teamPlayersService = teamPlayersService;
            _userService = userService;
            _playerService = playerService;
            _context = context;
        }

        private async Task<MatchDto> MapToDtoAsync(Match match)
        {
            var matchTeams = await _matchTeamsService.GetMatchTeamsByMatchIdAsync(match.Id);
            var teams = matchTeams.ToList();

            var playerHistory = new List<PlayerHistoryDto>();
            foreach (var matchTeam in teams)
            {
                var teamPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id);
                foreach (var teamPlayer in teamPlayers)
                {
                    if (teamPlayer.Player != null)
                    {
                        var playerDto = new PlayerDto
                        {
                            Id = teamPlayer.Player.Id,
                            FirstName = teamPlayer.Player.FirstName,
                            LastName = teamPlayer.Player.LastName,
                            Rating = teamPlayer.Player.Rating,
                            Speed = teamPlayer.Player.Speed,
                            Stamina = teamPlayer.Player.Stamina,
                            Errors = teamPlayer.Player.Errors,
                            UserEmail = teamPlayer.Player.UserEmail,
                            Username = teamPlayer.Player.Username
                        };

                        playerHistory.Add(new PlayerHistoryDto
                        {
                            PlayerId = teamPlayer.PlayerId,
                            TeamId = matchTeam.TeamId,
                            Status = teamPlayer.Status,
                            Player = playerDto
                        });
                    }
                }
            }

            return new MatchDto
            {
                Id = match.Id,
                MatchDate = match.MatchDate,
                IsPublic = match.IsPublic,
                Status = match.Status,
                Location = match.Location,
                Cost = match.Cost,
                OrganiserId = match.OrganiserId,
                TeamAName = teams.Count > 0 ? teams[0].Team?.Name : null,
                TeamBName = teams.Count > 1 ? teams[1].Team?.Name : null,
                TeamAId = teams.Count > 0 ? teams[0].TeamId : null,
                TeamBId = teams.Count > 1 ? teams[1].TeamId : null,
                ScoreA = teams.Count > 0 ? teams[0].Goals : null,
                ScoreB = teams.Count > 1 ? teams[1].Goals : null,
                PlayerHistory = playerHistory
            };
        }

        public async Task<IEnumerable<MatchDto>> GetAllMatchesAsync()
        {
            var matches = await _matchRepository.GetAllAsync();
            var matchDtos = new List<MatchDto>();
            foreach (var match in matches)
            {
                matchDtos.Add(await MapToDtoAsync(match));
            }
            return matchDtos;
        }

        public async Task<MatchDto> GetMatchByIdAsync(int id)
        {
            var match = await _matchRepository.GetByIdAsync(id);
            return match == null ? null : await MapToDtoAsync(match);
        }

        public async Task<IEnumerable<MatchDto>> GetMatchesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var matches = await _matchRepository.GetMatchesByDateRangeAsync(startDate, endDate);
            var matchDtos = new List<MatchDto>();
            foreach (var match in matches)
            {
                matchDtos.Add(await MapToDtoAsync(match));
            }
            return matchDtos;
        }

        public async Task<IEnumerable<MatchDto>> GetPublicMatchesAsync()
        {
            var matches = await _matchRepository.GetPublicMatchesAsync();
            var matchDtos = new List<MatchDto>();
            foreach (var match in matches)
            {
                matchDtos.Add(await MapToDtoAsync(match));
            }
            return matchDtos;
        }

        public async Task<IEnumerable<MatchDto>> GetMatchesByStatusAsync(Status status)
        {
            var matches = await _matchRepository.GetMatchesByStatusAsync(status);
            var matchDtos = new List<MatchDto>();
            foreach (var match in matches)
            {
                matchDtos.Add(await MapToDtoAsync(match));
            }
            return matchDtos;
        }

        public async Task<MatchDto> CreateMatchAsync(CreateMatchDto createMatchDto, int organiserId)
        {
            var match = new Match
            {
                MatchDate = DateTime.SpecifyKind(createMatchDto.MatchDate, DateTimeKind.Utc),
                IsPublic = false,
                Status = createMatchDto.Status,
                Location = createMatchDto.Location,
                Cost = createMatchDto.Cost,
                OrganiserId = organiserId
            };

            var createdMatch = await _matchRepository.CreateAsync(match);

            var teamADto = new CreateTeamDto { Name = createMatchDto.TeamAName ?? "TeamA" };
            var teamBDto = new CreateTeamDto { Name = createMatchDto.TeamBName ?? "TeamB" };

            var teamA = await _teamService.CreateTeamAsync(teamADto);
            var teamB = await _teamService.CreateTeamAsync(teamBDto);

            var matchTeamADto = new CreateMatchTeamsDto
            {
                MatchId = createdMatch.Id,
                TeamId = teamA.Id,
                Goals = 0
            };

            var matchTeamBDto = new CreateMatchTeamsDto
            {
                MatchId = createdMatch.Id,
                TeamId = teamB.Id,
                Goals = 0
            };

            await _matchTeamsService.CreateMatchTeamAsync(matchTeamADto);
            await _matchTeamsService.CreateMatchTeamAsync(matchTeamBDto);

            return await GetMatchByIdAsync(createdMatch.Id);
        }

        public async Task<MatchDto> MakeMatchPublicAsync(int matchId)
        {
            var existingMatch = await _matchRepository.GetByIdAsync(matchId);
            if (existingMatch == null)
                return null;

            existingMatch.IsPublic = true;
            var updatedMatch = await _matchRepository.UpdateAsync(existingMatch);
            return await MapToDtoAsync(updatedMatch);
        }

        public async Task<MatchDto> MakeMatchPrivateAsync(int matchId)
        {
            var existingMatch = await _matchRepository.GetByIdAsync(matchId);
            if (existingMatch == null)
                return null;

            existingMatch.IsPublic = false;
            var updatedMatch = await _matchRepository.UpdateAsync(existingMatch);
            return await MapToDtoAsync(updatedMatch);
        }

        public async Task<MatchDto> UpdateMatchAsync(int id, UpdateMatchDto updateMatchDto)
        {
            var existingMatch = await _matchRepository.GetByIdAsync(id);
            if (existingMatch == null)
                return null;

            existingMatch.MatchDate = DateTime.SpecifyKind(updateMatchDto.MatchDate, DateTimeKind.Utc);
            existingMatch.IsPublic = updateMatchDto.IsPublic;
            existingMatch.Status = updateMatchDto.Status;
            existingMatch.Location = updateMatchDto.Location;
            existingMatch.Cost = updateMatchDto.Cost;

            var updatedMatch = await _matchRepository.UpdateAsync(existingMatch);
            return await MapToDtoAsync(updatedMatch);
        }

        public async Task<bool> DeleteMatchAsync(int id)
        {
            return await _matchRepository.DeleteAsync(id);
        }
        public async Task<IEnumerable<MatchDto>> GetFutureMatchesAsync()
        {
            var futureMatches = await _matchRepository.GetFutureMatchesAsync();
            var matchDtos = new List<MatchDto>();
            foreach (var match in futureMatches)
            {
                matchDtos.Add(await MapToDtoAsync(match));
            }
            return matchDtos;

        }
        public async Task<IEnumerable<MatchDto>> GetPastMatchesAsync()
        {
            var pastMatches = await _matchRepository.GetPastMatchesAsync();
            var matchDtos = new List<MatchDto>();
            foreach (var match in pastMatches)
            {
                matchDtos.Add(await MapToDtoAsync(match));
            }
            return matchDtos;
        }

        public async Task<IEnumerable<MatchDto>> GetMatchesByOrganiserAsync(int organiserId)
        {
            var matches = await _matchRepository.GetMatchesByOrganiserAsync(organiserId);
            var matchDtos = new List<MatchDto>();
            foreach (var match in matches)
            {
                matchDtos.Add(await MapToDtoAsync(match));
            }
            return matchDtos;
        }

        public async Task<IEnumerable<MatchDto>> GetMatchesByLocationAsync(string location)
        {
            var matches = await _matchRepository.GetMatchesByLocationAsync(location);
            var matchDtos = new List<MatchDto>();
            foreach (var match in matches)
            {
                matchDtos.Add(await MapToDtoAsync(match));
            }
            return matchDtos;
        }

        public async Task<IEnumerable<MatchDto>> GetMatchesByCostRangeAsync(decimal? minCost, decimal? maxCost)
        {
            var matches = await _matchRepository.GetMatchesByCostRangeAsync(minCost, maxCost);
            var matchDtos = new List<MatchDto>();
            foreach (var match in matches)
            {
                matchDtos.Add(await MapToDtoAsync(match));
            }
            return matchDtos;
        }

        public async Task<bool> AddPlayerToTeamAsync(int matchId, int playerId, int teamId)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null) return false;

            var organiserPlayers = await _userService.GetPlayersByOrganiserAsync(match.OrganiserId);
            if (!organiserPlayers.Any(p => p.Id == playerId))
                return false;

            var matchTeam = await _matchTeamsService.GetMatchTeamByMatchIdAndTeamIdAsync(matchId, teamId);
            if (matchTeam == null) return false;

            var existingPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id);
            if (existingPlayers.Count() >= 6) return false;

            var existingPlayer = await _teamPlayersService.GetTeamPlayerByMatchTeamIdAndPlayerIdAsync(matchTeam.Id, playerId);
            if (existingPlayer != null) return false;

            var createTeamPlayerDto = new CreateTeamPlayersDto
            {
                MatchTeamId = matchTeam.Id,
                PlayerId = playerId,
                Status = PlayerStatus.AddedByOrganiser
            };

            var result = await _teamPlayersService.CreateTeamPlayerAsync(createTeamPlayerDto);
            return result != null;
        }

        public async Task<bool> JoinSpecificTeamAsync(int matchId, int userId, int teamId)
        {
            var realPlayerId = await _playerService.GetPlayerIdByUserIdAsync(userId);
            if (realPlayerId == null) return false;

            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null || !match.IsPublic) return false;

            var matchTeam = await _matchTeamsService.GetMatchTeamByMatchIdAndTeamIdAsync(matchId, teamId);
            if (matchTeam == null) return false;

            var existingPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id);
            if (existingPlayers.Count() >= 6) return false;

            var existingPlayer = await _teamPlayersService.GetTeamPlayerByMatchTeamIdAndPlayerIdAsync(matchTeam.Id, realPlayerId.Value);
            if (existingPlayer != null) return false;

            var allMatchTeams = await _matchTeamsService.GetMatchTeamsByMatchIdAsync(matchId);
            foreach (var mt in allMatchTeams)
            {
                var playersInTeam = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(mt.Id);
                if (playersInTeam.Any(p => p.PlayerId == realPlayerId.Value))
                    return false;
            }

            var createTeamPlayerDto = new CreateTeamPlayersDto
            {
                MatchTeamId = matchTeam.Id,
                PlayerId = realPlayerId.Value,
                Status = PlayerStatus.Joined
            };

            var result = await _teamPlayersService.CreateTeamPlayerAsync(createTeamPlayerDto);
            return result != null;
        }

        public async Task<bool> JoinPublicMatchAsync(int matchId, int userId)
        {
            var realPlayerId = await _playerService.GetPlayerIdByUserIdAsync(userId);
            if (realPlayerId == null) return false;

            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null || !match.IsPublic) return false;

            var matchTeams = await _matchTeamsService.GetMatchTeamsByMatchIdAsync(matchId);
            if (matchTeams.Count() != 2) return false;

            var teamACounts = 0;
            var teamBCounts = 0;
            var teamAId = matchTeams.First().Id;
            var teamBId = matchTeams.Last().Id;

            var teamAPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(teamAId);
            var teamBPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(teamBId);

            teamACounts = teamAPlayers.Count();
            teamBCounts = teamBPlayers.Count();

            if (teamACounts + teamBCounts >= 12) return false;

            var allPlayers = teamAPlayers.Concat(teamBPlayers);
            if (allPlayers.Any(p => p.PlayerId == realPlayerId)) return false;

            int targetTeamId = teamBCounts < teamACounts ? teamBId : (teamACounts == teamBCounts ? teamBId : teamAId);

            var createTeamPlayerDto = new CreateTeamPlayersDto
            {
                MatchTeamId = targetTeamId,
                PlayerId = realPlayerId.Value,
                Status = PlayerStatus.Joined
            };

            var result = await _teamPlayersService.CreateTeamPlayerAsync(createTeamPlayerDto);
            return result != null;
        }

        public async Task<bool> MovePlayerBetweenTeamsAsync(int matchId, int playerId, int newTeamId)
        {
            var matchTeams = await _matchTeamsService.GetMatchTeamsByMatchIdAsync(matchId);
            var newMatchTeam = matchTeams.FirstOrDefault(mt => mt.TeamId == newTeamId);
            if (newMatchTeam == null) return false;

            var existingPlayersInNewTeam = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(newMatchTeam.Id);
            if (existingPlayersInNewTeam.Count() >= 6) return false;

            TeamPlayersDto currentPlayerEntry = null;
            foreach (var matchTeam in matchTeams)
            {
                var playersInTeam = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id);
                currentPlayerEntry = playersInTeam.FirstOrDefault(tp => tp.PlayerId == playerId);
                if (currentPlayerEntry != null) break;
            }

            if (currentPlayerEntry == null) return false;

            await _teamPlayersService.DeleteTeamPlayerAsync(currentPlayerEntry.Id);

            var createTeamPlayerDto = new CreateTeamPlayersDto
            {
                MatchTeamId = newMatchTeam.Id,
                PlayerId = playerId,
                Status = currentPlayerEntry.Status
            };

            var result = await _teamPlayersService.CreateTeamPlayerAsync(createTeamPlayerDto);
            return result != null;
        }

        public async Task<MatchDto> PublishMatchAsync(int matchId)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null) return null;

            match.IsPublic = true;
            await _matchRepository.UpdateAsync(match);

            return await MapToDtoAsync(match);
        }

        public async Task<MatchDetailsDto> GetMatchDetailsAsync(int matchId)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null) return null;

            var matchTeams = await _matchTeamsService.GetMatchTeamsByMatchIdAsync(matchId);
            var teams = new List<TeamWithPlayersDto>();
            var totalPlayers = 0;

            foreach (var matchTeam in matchTeams)
            {
                var teamPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id);
                var playersDto = teamPlayers.Select(tp => new PlayerInMatchDto
                {
                    Id = tp.Player?.Id ?? 0,
                    PlayerId = tp.PlayerId,
                    FirstName = tp.Player?.FirstName ?? "",
                    LastName = tp.Player?.LastName ?? "",
                    PlayerName = $"{tp.Player?.FirstName ?? ""} {tp.Player?.LastName ?? ""}".Trim(),
                    Username = tp.Player?.Username ?? "Unknown",
                    Rating = (decimal)(tp.Player?.Rating ?? 0f),
                    Speed = tp.Player?.Speed ?? 2,
                    Stamina = tp.Player?.Stamina ?? 2,
                    Errors = tp.Player?.Errors ?? 2,
                    Status = tp.Status
                }).ToList();

                teams.Add(new TeamWithPlayersDto
                {
                    TeamId = matchTeam.TeamId,
                    TeamName = matchTeam.Team?.Name ?? "Team",
                    MatchTeamId = matchTeam.Id,
                    Players = playersDto
                });

                totalPlayers += playersDto.Count;
            }

            return new MatchDetailsDto
            {
                Id = match.Id,
                MatchDate = match.MatchDate,
                IsPublic = match.IsPublic,
                Status = match.Status,
                Location = match.Location,
                Cost = match.Cost,
                OrganiserId = match.OrganiserId,
                Teams = teams,
                TotalPlayers = totalPlayers
            };
        }

        public async Task<bool> LeaveMatchAsync(int matchId, int playerId)
        {
            var matchTeams = await _matchTeamsService.GetMatchTeamsByMatchIdAsync(matchId);

            foreach (var matchTeam in matchTeams)
            {
                var teamPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id);
                var playerInTeam = teamPlayers.FirstOrDefault(tp => tp.PlayerId == playerId);

                if (playerInTeam != null)
                {
                    return await _teamPlayersService.DeleteTeamPlayerAsync(playerInTeam.Id);
                }
            }

            return false;
        }

        public async Task<IEnumerable<MatchDto>> GetPlayerMatchesAsync(int playerId)
        {
            var playerMatches = new List<Match>();
            var allMatches = await _matchRepository.GetAllAsync();

            foreach (var match in allMatches)
            {
                var matchTeams = await _matchTeamsService.GetMatchTeamsByMatchIdAsync(match.Id);

                foreach (var matchTeam in matchTeams)
                {
                    var teamPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id);
                    if (teamPlayers.Any(tp => tp.PlayerId == playerId))
                    {
                        playerMatches.Add(match);
                        break;
                    }
                }
            }

            var matchDtos = new List<MatchDto>();
            foreach (var match in playerMatches)
            {
                matchDtos.Add(await MapToDtoAsync(match));
            }
            return matchDtos;
        }

        public async Task<IEnumerable<MatchDto>> GetAvailableMatchesForPlayerAsync(int playerId)
        {
            var availableMatches = new List<Match>();

            var allMatches = await _matchRepository.GetAllAsync();
            var futureMatches = allMatches.Where(m => m.MatchDate > DateTime.Now).ToList();

            foreach (var match in futureMatches)
            {
                var matchTeams = await _matchTeamsService.GetMatchTeamsByMatchIdAsync(match.Id);
                bool playerAlreadyInMatch = false;

                foreach (var matchTeam in matchTeams)
                {
                    var teamPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id);
                    if (teamPlayers.Any(tp => tp.PlayerId == playerId))
                    {
                        playerAlreadyInMatch = true;
                        break;
                    }
                }

                if (!playerAlreadyInMatch)
                {
                    if (match.IsPublic)
                    {
                        availableMatches.Add(match);
                    }
                }
            }

            var matchDtos = new List<MatchDto>();
            foreach (var match in availableMatches)
            {
                matchDtos.Add(await MapToDtoAsync(match));
            }

            return matchDtos;
        }

        public async Task<bool> RemovePlayerFromMatchAsync(int matchId, int playerId)
        {
            var matchTeams = await _matchTeamsService.GetMatchTeamsByMatchIdAsync(matchId);

            foreach (var matchTeam in matchTeams)
            {
                var teamPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id);
                var playerInTeam = teamPlayers.FirstOrDefault(tp => tp.PlayerId == playerId);

                if (playerInTeam != null)
                {
                    return await _teamPlayersService.DeleteTeamPlayerAsync(playerInTeam.Id);
                }
            }

            return false;
        }

    }
}