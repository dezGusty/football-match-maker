using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Models.Enums;
using FootballAPI.Repository;

namespace FootballAPI.Service
{
    public class TeamPlayersService : ITeamPlayersService
    {
        private readonly ITeamPlayersRepository _teamPlayersRepository;
        private readonly IMatchTeamsRepository _matchTeamsRepository;
        private readonly IUserRepository _userRepository;

        public TeamPlayersService(ITeamPlayersRepository teamPlayersRepository,
                                 IMatchTeamsRepository matchTeamsRepository,
                                 IUserRepository userRepository)
        {
            _teamPlayersRepository = teamPlayersRepository;
            _matchTeamsRepository = matchTeamsRepository;
            _userRepository = userRepository;
        }

        public async Task<TeamPlayersDto> CreateTeamPlayerAsync(CreateTeamPlayersDto createTeamPlayersDto)
        {
            var teamPlayer = new TeamPlayers
            {
                MatchTeamId = createTeamPlayersDto.MatchTeamId,
                UserId = createTeamPlayersDto.UserId,
                Status = createTeamPlayersDto.Status
            };

            var createdTeamPlayer = await _teamPlayersRepository.CreateAsync(teamPlayer);
            return MapToDto(createdTeamPlayer);
        }

        public async Task<bool> DeleteTeamPlayerAsync(int id)
        {
            return await _teamPlayersRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<TeamPlayersDto>> GetTeamPlayersByMatchTeamIdAsync(int matchTeamId)
        {
            var teamPlayers = await _teamPlayersRepository.GetByMatchTeamIdAsync(matchTeamId);
            return teamPlayers.Select(MapToDto);
        }

        public async Task<TeamPlayersDto> GetTeamPlayerByMatchTeamIdAndUserIdAsync(int matchTeamId, int userId)
        {
            var teamPlayer = await _teamPlayersRepository.GetByMatchTeamIdAndUserIdAsync(matchTeamId, userId);
            return teamPlayer == null ? null : MapToDto(teamPlayer);
        }

        private TeamPlayersDto MapToDto(TeamPlayers teamPlayer)
        {
            return new TeamPlayersDto
            {
                Id = teamPlayer.Id,
                MatchTeamId = teamPlayer.MatchTeamId,
                UserId = teamPlayer.UserId,
                Status = teamPlayer.Status,
                MatchTeam = teamPlayer.MatchTeam == null ? null : new MatchTeamsDto
                {
                    Id = teamPlayer.MatchTeam.Id,
                    MatchId = teamPlayer.MatchTeam.MatchId,
                    TeamId = teamPlayer.MatchTeam.TeamId,
                    Goals = teamPlayer.MatchTeam.Goals,
                    Match = teamPlayer.MatchTeam.Match == null ? null : new MatchDto
                    {
                        Id = teamPlayer.MatchTeam.Match.Id,
                        MatchDate = teamPlayer.MatchTeam.Match.MatchDate,
                        IsPublic = teamPlayer.MatchTeam.Match.IsPublic,
                        Status = teamPlayer.MatchTeam.Match.Status
                    },
                    Team = teamPlayer.MatchTeam.Team == null ? null : new TeamDto
                    {
                        Id = teamPlayer.MatchTeam.Team.Id,
                        Name = teamPlayer.MatchTeam.Team.Name
                    }
                },
                User = teamPlayer.User == null ? null : new UserDto
                {
                    Id = teamPlayer.User.Id,
                    FirstName = teamPlayer.User.FirstName,
                    LastName = teamPlayer.User.LastName,
                    Rating = teamPlayer.User.Rating,
                    Speed = teamPlayer.User.Speed,
                    Stamina = teamPlayer.User.Stamina,
                    Errors = teamPlayer.User.Errors,
                    Email = teamPlayer.User?.Email,
                    Username = teamPlayer.User?.Username,
                    IsDeleted = teamPlayer.User.DeletedAt != null,
                    CreatedAt = teamPlayer.User.CreatedAt,
                    UpdatedAt = teamPlayer.User.UpdatedAt,
                    DeletedAt = teamPlayer.User.DeletedAt
                }
            };
        }
    }
}