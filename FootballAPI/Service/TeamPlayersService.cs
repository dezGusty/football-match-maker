using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository;

namespace FootballAPI.Service
{
    public class TeamPlayersService : ITeamPlayersService
    {
        private readonly ITeamPlayersRepository _teamPlayersRepository;
        private readonly IMatchTeamsRepository _matchTeamsRepository;
        private readonly IPlayerRepository _playerRepository;

        public TeamPlayersService(ITeamPlayersRepository teamPlayersRepository,
                                 IMatchTeamsRepository matchTeamsRepository,
                                 IPlayerRepository playerRepository)
        {
            _teamPlayersRepository = teamPlayersRepository;
            _matchTeamsRepository = matchTeamsRepository;
            _playerRepository = playerRepository;
        }

        public async Task<IEnumerable<TeamPlayersDto>> GetAllTeamPlayersAsync()
        {
            var teamPlayers = await _teamPlayersRepository.GetAllAsync();
            return teamPlayers.Select(MapToDto);
        }

        public async Task<TeamPlayersDto> GetTeamPlayerByIdAsync(int id)
        {
            var teamPlayer = await _teamPlayersRepository.GetByIdAsync(id);
            return teamPlayer == null ? null : MapToDto(teamPlayer);
        }

        public async Task<TeamPlayersDto> CreateTeamPlayerAsync(CreateTeamPlayersDto createTeamPlayersDto)
        {
            var teamPlayer = new TeamPlayers
            {
                MatchTeamId = createTeamPlayersDto.MatchTeamId,
                PlayerId = createTeamPlayersDto.PlayerId,
                Status = createTeamPlayersDto.Status
            };

            var createdTeamPlayer = await _teamPlayersRepository.CreateAsync(teamPlayer);
            return MapToDto(createdTeamPlayer);
        }

        public async Task<TeamPlayersDto> UpdateTeamPlayerAsync(int id, UpdateTeamPlayersDto updateTeamPlayersDto)
        {
            var existingTeamPlayer = await _teamPlayersRepository.GetByIdAsync(id);
            if (existingTeamPlayer == null)
            {
                return null;
            }

            existingTeamPlayer.MatchTeamId = updateTeamPlayersDto.MatchTeamId;
            existingTeamPlayer.PlayerId = updateTeamPlayersDto.PlayerId;
            existingTeamPlayer.Status = updateTeamPlayersDto.Status;

            var updatedTeamPlayer = await _teamPlayersRepository.UpdateAsync(existingTeamPlayer);
            return MapToDto(updatedTeamPlayer);
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

        public async Task<IEnumerable<TeamPlayersDto>> GetTeamPlayersByPlayerIdAsync(int playerId)
        {
            var teamPlayers = await _teamPlayersRepository.GetByPlayerIdAsync(playerId);
            return teamPlayers.Select(MapToDto);
        }

        public async Task<TeamPlayersDto> GetTeamPlayerByMatchTeamIdAndPlayerIdAsync(int matchTeamId, int playerId)
        {
            var teamPlayer = await _teamPlayersRepository.GetByMatchTeamIdAndPlayerIdAsync(matchTeamId, playerId);
            return teamPlayer == null ? null : MapToDto(teamPlayer);
        }

        public async Task<IEnumerable<TeamPlayersDto>> GetTeamPlayersByStatusAsync(PlayerStatus status)
        {
            var teamPlayers = await _teamPlayersRepository.GetByStatusAsync(status);
            return teamPlayers.Select(MapToDto);
        }

        private TeamPlayersDto MapToDto(TeamPlayers teamPlayer)
        {
            return new TeamPlayersDto
            {
                Id = teamPlayer.Id,
                MatchTeamId = teamPlayer.MatchTeamId,
                PlayerId = teamPlayer.PlayerId,
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
                Player = teamPlayer.Player == null ? null : new PlayerDto
                {
                    Id = teamPlayer.Player.Id,
                    FirstName = teamPlayer.Player.FirstName,
                    LastName = teamPlayer.Player.LastName,
                    Rating = teamPlayer.Player.Rating,
                    IsDeleted = teamPlayer.Player.DeletedAt != null,
                    CreatedAt = teamPlayer.Player.CreatedAt,
                    UpdatedAt = teamPlayer.Player.UpdatedAt,
                    DeletedAt = teamPlayer.Player.DeletedAt
                }
            };
        }
    }
}