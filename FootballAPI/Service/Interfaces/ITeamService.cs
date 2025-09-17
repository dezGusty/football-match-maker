using FootballAPI.Models.Enums;
using FootballAPI.DTOs;

namespace FootballAPI.Service
{
    public interface ITeamService
    {
        Task<TeamDto> GetTeamByIdAsync(int id);
        Task<TeamDto> CreateTeamAsync(CreateTeamDto createTeamDto);
        Task<TeamDto> UpdateTeamAsync(int id, UpdateTeamDto updateTeamDto);
    }
}