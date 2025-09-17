using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository;

namespace FootballAPI.Service
{
    public class MatchTeamsService : IMatchTeamsService
    {
        private readonly IMatchTeamsRepository _matchTeamsRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamRepository _teamRepository;

        public MatchTeamsService(IMatchTeamsRepository matchTeamsRepository,
                                IMatchRepository matchRepository,
                                ITeamRepository teamRepository)
        {
            _matchTeamsRepository = matchTeamsRepository;
            _matchRepository = matchRepository;
            _teamRepository = teamRepository;
        }

        public async Task<IEnumerable<MatchTeamsDto>> GetAllMatchTeamsAsync()
        {
            var matchTeams = await _matchTeamsRepository.GetAllAsync();
            return matchTeams.Select(MapToDto);
        }

        public async Task<MatchTeamsDto> GetMatchTeamByIdAsync(int id)
        {
            var matchTeam = await _matchTeamsRepository.GetByIdAsync(id);
            return matchTeam == null ? null : MapToDto(matchTeam);
        }

        public async Task<MatchTeamsDto> CreateMatchTeamAsync(CreateMatchTeamsDto createMatchTeamsDto)
        {
            var matchTeam = new MatchTeams
            {
                MatchId = createMatchTeamsDto.MatchId,
                TeamId = createMatchTeamsDto.TeamId,
                Goals = createMatchTeamsDto.Goals
            };

            var createdMatchTeam = await _matchTeamsRepository.CreateAsync(matchTeam);
            return MapToDto(createdMatchTeam);
        }

        public async Task<MatchTeamsDto> UpdateMatchTeamAsync(int id, UpdateMatchTeamsDto updateMatchTeamsDto)
        {
            var existingMatchTeam = await _matchTeamsRepository.GetByIdAsync(id);
            if (existingMatchTeam == null)
            {
                return null;
            }

            existingMatchTeam.MatchId = updateMatchTeamsDto.MatchId;
            existingMatchTeam.TeamId = updateMatchTeamsDto.TeamId;
            existingMatchTeam.Goals = updateMatchTeamsDto.Goals;

            var updatedMatchTeam = await _matchTeamsRepository.UpdateAsync(existingMatchTeam);
            return MapToDto(updatedMatchTeam);
        }

        public async Task<IEnumerable<MatchTeamsDto>> GetMatchTeamsByMatchIdAsync(int matchId)
        {
            var matchTeams = await _matchTeamsRepository.GetByMatchIdAsync(matchId);
            return matchTeams.Select(MapToDto);
        }

        public async Task<MatchTeamsDto> GetMatchTeamByMatchIdAndTeamIdAsync(int matchId, int teamId)
        {
            var matchTeam = await _matchTeamsRepository.GetByMatchIdAndTeamIdAsync(matchId, teamId);
            return matchTeam == null ? null : MapToDto(matchTeam);
        }

        private MatchTeamsDto MapToDto(MatchTeams matchTeam)
        {
            return new MatchTeamsDto
            {
                Id = matchTeam.Id,
                MatchId = matchTeam.MatchId,
                TeamId = matchTeam.TeamId,
                Goals = matchTeam.Goals,
                Match = matchTeam.Match == null ? null : new MatchDto
                {
                    Id = matchTeam.Match.Id,
                    MatchDate = matchTeam.Match.MatchDate,
                    IsPublic = matchTeam.Match.IsPublic,
                    Status = matchTeam.Match.Status
                },
                Team = matchTeam.Team == null ? null : new TeamDto
                {
                    Id = matchTeam.Team.Id,
                    Name = matchTeam.Team.Name
                },

            };
        }
    }
}