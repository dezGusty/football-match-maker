using FootballAPI.Models.Enums;
using FootballAPI.Models;

namespace FootballAPI.Repository
{
    public interface ITeamRepository
    {
        Task<Team> GetByIdAsync(int id);
        Task<Team> CreateAsync(Team team);
        Task<Team> UpdateAsync(Team team);
    }
}