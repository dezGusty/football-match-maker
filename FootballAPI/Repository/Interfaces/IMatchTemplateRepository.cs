using FootballAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootballAPI.Repository.Interfaces
{
    public interface IMatchTemplateRepository
    {
        Task<IEnumerable<MatchTemplate>> GetAllByUserIdAsync(int userId);
        Task<MatchTemplate> GetByIdAsync(int id);
        Task<MatchTemplate> CreateAsync(MatchTemplate matchTemplate);
        Task<MatchTemplate> UpdateAsync(MatchTemplate matchTemplate);
        Task<bool> DeleteAsync(int id);
    }
}
