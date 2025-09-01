using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Models.Enums;
using FootballAPI.Repository;
using FootballAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FootballAPI.Service
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamService _teamService;
        private readonly IMatchTeamsService _matchTeamsService;
        private readonly ITeamPlayersService _teamPlayersService;
        private readonly IUserService _userService;
        private readonly FootballDbContext _context;

        public MatchService(IMatchRepository matchRepository, ITeamService teamService,
                           IMatchTeamsService matchTeamsService, ITeamPlayersService teamPlayersService,
                           IUserService userService, FootballDbContext context)
        {
            _matchRepository = matchRepository;
            _teamService = teamService;
            _matchTeamsService = matchTeamsService;
            _teamPlayersService = teamPlayersService;
            _userService = userService;
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
                    if (teamPlayer.User != null)
                    {
                        var playerDto = new UserDto
                        {
                            Id = teamPlayer.User.Id,
                            FirstName = teamPlayer.User.FirstName,
                            LastName = teamPlayer.User.LastName,
                            Rating = teamPlayer.User.Rating,
                            Speed = teamPlayer.User.Speed,
                            Stamina = teamPlayer.User.Stamina,
                            Errors = teamPlayer.User.Errors,
                            Email = teamPlayer.User.Email,
                            Username = teamPlayer.User.Username
                        };

                        playerHistory.Add(new PlayerHistoryDto
                        {
                            UserId = teamPlayer.UserId,
                            TeamId = matchTeam.TeamId,
                            Status = teamPlayer.Status,
                            User = playerDto
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
            var matchDate = DateTime.Parse(createMatchDto.MatchDate);

            if (matchDate <= DateTime.Now)
            {
                throw new ArgumentException("Cannot create a match in the past. Please select a future date and time.");
            }

            var match = new Match
            {
                MatchDate = matchDate,
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

            var matchDate = DateTime.Parse(updateMatchDto.MatchDate);

            if (matchDate <= DateTime.Now)
            {
                throw new ArgumentException("Cannot update match to a past date. Please select a future date and time.");
            }

            existingMatch.MatchDate = matchDate;
            existingMatch.Location = updateMatchDto.Location;
            existingMatch.Cost = updateMatchDto.Cost;

            var matchTeams = await _matchTeamsService.GetMatchTeamsByMatchIdAsync(id);
            if (matchTeams.Count() != 2)
                throw new InvalidOperationException("Match must have exactly two teams.");

            var teamA = matchTeams.First();
            var teamB = matchTeams.Last();

            var winningTeam = teamA.Goals > teamB.Goals ? teamA : (teamB.Goals > teamA.Goals ? teamB : null);
            var losingTeam = winningTeam == teamA ? teamB : teamA;

            if (winningTeam != null)
            {
                var winningPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(winningTeam.Id);
                foreach (var player in winningPlayers)
                {
                    float playerErrorsChange = player.User.Errors switch
                    {
                        1 => 0.025f,
                        2 => 0.05f,
                        3 => 0.075f,
                        4 => 0.1f,
                        _ => 0f
                    };

                    float totalRating =
                        (player.User.Rating * 0.005f) +
                        (player.User.Speed * 0.025f) +
                        (player.User.Stamina * 0.025f) +
                        playerErrorsChange;

                    await _userService.UpdatePlayerRatingAsync(player.UserId, totalRating);
                    existingMatch.MatchDate = DateTime.Parse(updateMatchDto.MatchDate);
                    existingMatch.Location = updateMatchDto.Location;
                    existingMatch.Cost = updateMatchDto.Cost;

                    if (!string.IsNullOrEmpty(updateMatchDto.TeamAName) || !string.IsNullOrEmpty(updateMatchDto.TeamBName))
                    {
                        var teams = matchTeams.ToList();

                        if (!string.IsNullOrEmpty(updateMatchDto.TeamAName) && teams.Count > 0)
                        {
                            var updateTeamDto = new UpdateTeamDto { Name = updateMatchDto.TeamAName };
                            await _teamService.UpdateTeamAsync(teams[0].TeamId, updateTeamDto);
                        }

                        if (!string.IsNullOrEmpty(updateMatchDto.TeamBName) && teams.Count > 1)
                        {
                            var updateTeamDto = new UpdateTeamDto { Name = updateMatchDto.TeamBName };
                            await _teamService.UpdateTeamAsync(teams[1].TeamId, updateTeamDto);
                        }
                    }

                }

                var losingPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(losingTeam.Id);
                foreach (var player in losingPlayers)
                {
                    float playerErrorsChange = player.User.Errors switch
                    {
                        1 => 0.025f,
                        2 => 0.05f,
                        3 => 0.075f,
                        4 => 0.1f,
                        _ => 0f
                    };

                    float totalRating =
                        (player.User.Rating * 0.005f) +
                        (player.User.Speed * 0.025f) +
                        (player.User.Stamina * 0.025f) +
                        playerErrorsChange;

                    await _userService.UpdatePlayerRatingAsync(player.UserId, -totalRating);
                }
            }
            else
            {
                var teamAPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(teamA.Id);
                var teamBPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(teamB.Id);

                foreach (var player in teamAPlayers.Concat(teamBPlayers))
                {
                    float playerErrorsChange = player.User.Errors switch
                    {
                        1 => 0.025f,
                        2 => 0.05f,
                        3 => 0.075f,
                        4 => 0.1f,
                        _ => 0f
                    };

                    float totalRating =
                        (player.User.Rating * 0.005f) +
                        (player.User.Speed * 0.025f) +
                        (player.User.Stamina * 0.025f) +
                        playerErrorsChange;

                    await _userService.UpdatePlayerRatingAsync(player.UserId, totalRating / 2);
                }
            }

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

        public async Task<bool> AddPlayerToTeamAsync(int matchId, int userId, int teamId)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null) return false;

            var organiserPlayers = await _userService.GetPlayersByOrganiserAsync(match.OrganiserId);

            if (!organiserPlayers.Any(p => p.Id == userId) && userId != match.OrganiserId)
                return false;

            var matchTeam = await _matchTeamsService.GetMatchTeamByMatchIdAndTeamIdAsync(matchId, teamId);
            if (matchTeam == null) return false;

            var existingPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id);
            if (existingPlayers.Count() >= 6) return false;

            var existingPlayer = await _teamPlayersService.GetTeamPlayerByMatchTeamIdAndUserIdAsync(matchTeam.Id, userId);
            if (existingPlayer != null) return false;

            var createTeamUserDto = new CreateTeamPlayersDto
            {
                MatchTeamId = matchTeam.Id,
                UserId = userId,
                Status = PlayerStatus.AddedByOrganiser
            };

            var result = await _teamPlayersService.CreateTeamPlayerAsync(createTeamUserDto);
            return result != null;
        }

        public async Task<bool> JoinSpecificTeamAsync(int matchId, int userId, int teamId)
        {

            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null || !match.IsPublic) return false;

            var matchTeam = await _matchTeamsService.GetMatchTeamByMatchIdAndTeamIdAsync(matchId, teamId);
            if (matchTeam == null) return false;

            var existingPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id);
            if (existingPlayers.Count() >= 6) return false;

            var existingPlayer = await _teamPlayersService.GetTeamPlayerByMatchTeamIdAndUserIdAsync(matchTeam.Id, userId);
            if (existingPlayer != null) return false;

            var allMatchTeams = await _matchTeamsService.GetMatchTeamsByMatchIdAsync(matchId);
            foreach (var mt in allMatchTeams)
            {
                var playersInTeam = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(mt.Id);
                if (playersInTeam.Any(p => p.UserId == userId))
                    return false;
            }

            var createTeamUserDto = new CreateTeamPlayersDto
            {
                MatchTeamId = matchTeam.Id,
                UserId = userId,
                Status = PlayerStatus.Joined
            };

            var result = await _teamPlayersService.CreateTeamPlayerAsync(createTeamUserDto);
            return result != null;
        }

        public async Task<bool> JoinPublicMatchAsync(int matchId, int userId)
        {

            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null || !match.IsPublic || match.Status != Status.Open) return false;

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
            if (allPlayers.Any(p => p.UserId == userId)) return false;

            int targetTeamId = teamBCounts < teamACounts ? teamBId : (teamACounts == teamBCounts ? teamBId : teamAId);

            var createTeamUserDto = new CreateTeamPlayersDto
            {
                MatchTeamId = targetTeamId,
                UserId = userId,
                Status = PlayerStatus.Joined
            };

            var result = await _teamPlayersService.CreateTeamPlayerAsync(createTeamUserDto);
            return result != null;
        }

        public async Task<bool> MovePlayerBetweenTeamsAsync(int matchId, int userId, int newTeamId)
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
                currentPlayerEntry = playersInTeam.FirstOrDefault(tp => tp.UserId == userId);
                if (currentPlayerEntry != null) break;
            }

            if (currentPlayerEntry == null) return false;

            await _teamPlayersService.DeleteTeamPlayerAsync(currentPlayerEntry.Id);

            var createTeamUserDto = new CreateTeamPlayersDto
            {
                MatchTeamId = newMatchTeam.Id,
                UserId = userId,
                Status = currentPlayerEntry.Status
            };

            var result = await _teamPlayersService.CreateTeamPlayerAsync(createTeamUserDto);
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
                    Id = tp.User?.Id ?? 0,
                    UserId = tp.UserId,
                    FirstName = tp.User?.FirstName ?? "",
                    LastName = tp.User?.LastName ?? "",
                    PlayerName = $"{tp.User?.FirstName ?? ""} {tp.User?.LastName ?? ""}".Trim(),
                    Username = tp.User?.Username ?? "Unknown",
                    Rating = (decimal)(tp.User?.Rating ?? 0f),
                    Speed = tp.User?.Speed ?? 2,
                    Stamina = tp.User?.Stamina ?? 2,
                    Errors = tp.User?.Errors ?? 2,
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

        public async Task<bool> LeaveMatchAsync(int matchId, int userId)
        {
            var matchTeams = await _matchTeamsService.GetMatchTeamsByMatchIdAsync(matchId);

            foreach (var matchTeam in matchTeams)
            {
                var teamPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id);
                var playerInTeam = teamPlayers.FirstOrDefault(tp => tp.UserId == userId);

                if (playerInTeam != null)
                {
                    return await _teamPlayersService.DeleteTeamPlayerAsync(playerInTeam.Id);
                }
            }

            return false;
        }

        public async Task<IEnumerable<MatchDto>> GetPlayerMatchesAsync(int userId)
        {
            var playerMatches = new List<Match>();
            var allMatches = await _matchRepository.GetAllAsync();

            foreach (var match in allMatches)
            {
                var matchTeams = await _matchTeamsService.GetMatchTeamsByMatchIdAsync(match.Id);

                foreach (var matchTeam in matchTeams)
                {
                    var teamPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id);
                    if (teamPlayers.Any(tp => tp.UserId == userId))
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

        public async Task<IEnumerable<MatchDto>> GetAvailableMatchesForPlayerAsync(int userId)
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
                    if (teamPlayers.Any(tp => tp.UserId == userId))
                    {
                        playerAlreadyInMatch = true;
                        break;
                    }
                }

                if (!playerAlreadyInMatch)
                {
                    if (match.IsPublic && match.Status == Status.Open)
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

        public async Task<bool> RemovePlayerFromMatchAsync(int matchId, int userId)
        {
            var matchTeams = await _matchTeamsService.GetMatchTeamsByMatchIdAsync(matchId);

            foreach (var matchTeam in matchTeams)
            {
                var teamPlayers = await _teamPlayersService.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id);
                var playerInTeam = teamPlayers.FirstOrDefault(tp => tp.UserId == userId);

                if (playerInTeam != null)
                {
                    return await _teamPlayersService.DeleteTeamPlayerAsync(playerInTeam.Id);
                }
            }

            return false;
        }

        public async Task<int> GetEffectiveOrganizerId(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            var sentDelegation = await _context.OrganizerDelegates
                .FirstOrDefaultAsync(d => d.OriginalOrganizerId == userId && d.IsActive);

            if (sentDelegation != null)
            {
                return sentDelegation.DelegateUserId;
            }

            var receivedDelegation = await _context.OrganizerDelegates
                .FirstOrDefaultAsync(d => d.DelegateUserId == userId && d.IsActive);

            if (receivedDelegation != null)
            {
                return userId;
            }

            return userId;
        }

        public async Task<MatchDto> CloseMatchAsync(int matchId)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null || match.Status != Status.Open)
                return null;

            var matchDetails = await GetMatchDetailsAsync(matchId);
            if (matchDetails.TotalPlayers < 10)
                return null;

            match.Status = Status.Closed;
            await _matchRepository.UpdateAsync(match);
            await _context.SaveChangesAsync();

            return await MapToDtoAsync(match);
        }

        public async Task<MatchDto> CancelMatchAsync(int matchId)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
                return null;

            match.Status = Status.Cancelled;
            await _matchRepository.UpdateAsync(match);
            await _context.SaveChangesAsync();

            return await MapToDtoAsync(match);
        }

        public async Task<MatchDto> FinalizeMatchAsync(int matchId)
        {
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
                return null;

            if (match.MatchDate > DateTime.Now)
                return null;

            match.Status = Status.Finalized;
            await _matchRepository.UpdateAsync(match);
            await _context.SaveChangesAsync();

            return await MapToDtoAsync(match);
        }
    }
}