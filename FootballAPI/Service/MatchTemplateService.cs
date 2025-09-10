using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository.Interfaces;
using FootballAPI.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootballAPI.Service
{
    public class MatchTemplateService : IMatchTemplateService
    {
        private readonly IMatchTemplateRepository _matchTemplateRepository;

        public MatchTemplateService(IMatchTemplateRepository matchTemplateRepository)
        {
            _matchTemplateRepository = matchTemplateRepository;
        }

        public async Task<IEnumerable<MatchTemplateDto>> GetAllByUserIdAsync(int userId)
        {
            // Get templates by userId - the controller will determine if an admin
            // should see all templates or just their own
            var templates = await _matchTemplateRepository.GetAllByUserIdAsync(userId);
            return templates.Select(MapToDto);
        }
        
        public async Task<IEnumerable<MatchTemplateDto>> GetAllAsync()
        {
            // Get all templates for admins
            var templates = await _matchTemplateRepository.GetAllAsync();
            return templates.Select(MapToDto);
        }

        public async Task<MatchTemplateDto> GetByIdAsync(int id, int userId)
        {
            var template = await _matchTemplateRepository.GetByIdAsync(id);
            if (template == null || template.UserId != userId)
                return null;

            return MapToDto(template);
        }

        public async Task<MatchTemplateDto> CreateAsync(CreateMatchTemplateDto dto, int userId)
        {
            var template = new MatchTemplate
            {
                UserId = userId,
                Location = dto.Location,
                Cost = dto.Cost,
                Name = dto.Name,
                TeamAName = dto.TeamAName,
                TeamBName = dto.TeamBName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _matchTemplateRepository.CreateAsync(template);
            return MapToDto(created);
        }

        public async Task<MatchTemplateDto> UpdateAsync(int id, UpdateMatchTemplateDto dto, int userId)
        {
            var template = await _matchTemplateRepository.GetByIdAsync(id);
            if (template == null || template.UserId != userId)
                return null;

            template.Location = dto.Location;
            template.Cost = dto.Cost;
            template.Name = dto.Name;
            template.TeamAName = dto.TeamAName;
            template.TeamBName = dto.TeamBName;
            template.UpdatedAt = DateTime.UtcNow;

            var updated = await _matchTemplateRepository.UpdateAsync(template);
            return MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var template = await _matchTemplateRepository.GetByIdAsync(id);
            if (template == null || template.UserId != userId)
                return false;

            return await _matchTemplateRepository.DeleteAsync(id);
        }

        private MatchTemplateDto MapToDto(MatchTemplate template)
        {
            return new MatchTemplateDto
            {
                Id = template.Id,
                Location = template.Location,
                Cost = template.Cost,
                Name = template.Name,
                TeamAName = template.TeamAName,
                TeamBName = template.TeamBName
            };
        }
    }
}
