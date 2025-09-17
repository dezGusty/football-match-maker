using FootballAPI.Models.Enums;
using FootballAPI.DTOs;
using FootballAPI.Models;

namespace FootballAPI.Service
{
    public interface ITeamPlayersService
    {
        Task<TeamPlayersDto> CreateTeamPlayerAsync(CreateTeamPlayersDto createTeamPlayersDto);
        Task<bool> DeleteTeamPlayerAsync(int id);
        Task<IEnumerable<TeamPlayersDto>> GetTeamPlayersByMatchTeamIdAsync(int matchTeamId);
        Task<TeamPlayersDto> GetTeamPlayerByMatchTeamIdAndUserIdAsync(int matchTeamId, int userId);
    }
}