using FootballAPI.DTOs;
using FootballAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootballAPI.Service.Interfaces
{
    public interface IMatchTemplateService
    {
        Task<IEnumerable<MatchTemplateDto>> GetAllByUserIdAsync(int userId);
        Task<MatchTemplateDto> GetByIdAsync(int id, int userId);
        Task<MatchTemplateDto> CreateAsync(CreateMatchTemplateDto dto, int userId);
        Task<MatchTemplateDto> UpdateAsync(int id, UpdateMatchTemplateDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
