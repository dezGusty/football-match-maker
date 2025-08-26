using FootballAPI.Models.Enums;
using FootballAPI.DTOs;
using FootballAPI.Models;

namespace FootballAPI.Service
{
    public interface ITeamPlayersService
    {
        Task<IEnumerable<TeamPlayersDto>> GetAllTeamPlayersAsync();
        Task<TeamPlayersDto> GetTeamPlayerByIdAsync(int id);
        Task<TeamPlayersDto> CreateTeamPlayerAsync(CreateTeamPlayersDto createTeamPlayersDto);
        Task<TeamPlayersDto> UpdateTeamPlayerAsync(int id, UpdateTeamPlayersDto updateTeamPlayersDto);
        Task<bool> DeleteTeamPlayerAsync(int id);
        Task<IEnumerable<TeamPlayersDto>> GetTeamPlayersByMatchTeamIdAsync(int matchTeamId);
        Task<IEnumerable<TeamPlayersDto>> GetTeamPlayersByUserIdAsync(int userId);
        Task<TeamPlayersDto> GetTeamPlayerByMatchTeamIdAndUserIdAsync(int matchTeamId, int userId);
        Task<IEnumerable<TeamPlayersDto>> GetTeamPlayersByStatusAsync(PlayerStatus status);
    }
}