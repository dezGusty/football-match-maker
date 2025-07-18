using FootballAPI.DTOs;

namespace FootballAPI.Service
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamDto>> GetAllTeamsAsync();
        Task<TeamDto> GetTeamByIdAsync(int id);
        Task<TeamDto> CreateTeamAsync(CreateTeamDto createTeamDto);
        Task<TeamDto> UpdateTeamAsync(int id, UpdateTeamDto updateTeamDto);
        Task<bool> DeleteTeamAsync(int id);
        Task<TeamDto> GetTeamByNameAsync(string name);
        Task<IEnumerable<TeamDto>> SearchTeamsByNameAsync(string searchTerm);
    }
} 