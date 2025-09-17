using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository;

namespace FootballAPI.Service
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        private TeamDto MapToDto(Team team)
        {
            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name
            };
        }

        public async Task<TeamDto> GetTeamByIdAsync(int id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            return team == null ? null : MapToDto(team);
        }

        public async Task<TeamDto> CreateTeamAsync(CreateTeamDto createTeamDto)
        {
            var team = new Team
            {
                Name = createTeamDto.Name
            };

            var createdTeam = await _teamRepository.CreateAsync(team);
            return MapToDto(createdTeam);
        }

        public async Task<TeamDto> UpdateTeamAsync(int id, UpdateTeamDto updateTeamDto)
        {
            var existingTeam = await _teamRepository.GetByIdAsync(id);
            if (existingTeam == null)
                return null;

            existingTeam.Name = updateTeamDto.Name;

            var updatedTeam = await _teamRepository.UpdateAsync(existingTeam);
            return MapToDto(updatedTeam);
        }

    }
}