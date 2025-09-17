using FootballAPI.Data;
using FootballAPI.Models;
using FootballAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootballAPI.Repository
{
    public class MatchTemplateRepository : IMatchTemplateRepository
    {
        private readonly FootballDbContext _context;

        public MatchTemplateRepository(FootballDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MatchTemplate>> GetAllByUserIdAsync(int userId)
        {
            return await _context.MatchTemplates
                .Where(mt => mt.UserId == userId)
                .OrderByDescending(mt => mt.UpdatedAt)
                .ToListAsync();
        }

        public async Task<MatchTemplate> GetByIdAsync(int id)
        {
            return await _context.MatchTemplates.FindAsync(id);
        }

        public async Task<MatchTemplate> CreateAsync(MatchTemplate matchTemplate)
        {
            _context.MatchTemplates.Add(matchTemplate);
            await _context.SaveChangesAsync();
            return matchTemplate;
        }

        public async Task<MatchTemplate> UpdateAsync(MatchTemplate matchTemplate)
        {
            _context.Entry(matchTemplate).State = EntityState.Modified;
            matchTemplate.UpdatedAt = System.DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return matchTemplate;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var template = await _context.MatchTemplates.FindAsync(id);
            if (template == null)
                return false;

            _context.MatchTemplates.Remove(template);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
