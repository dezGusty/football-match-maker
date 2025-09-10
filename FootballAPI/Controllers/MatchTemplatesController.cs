using FootballAPI.DTOs;
using FootballAPI.Models.Enums;
using FootballAPI.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FootballAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MatchTemplatesController : ControllerBase
    {
        private readonly IMatchTemplateService _matchTemplateService;

        public MatchTemplatesController(IMatchTemplateService matchTemplateService)
        {
            _matchTemplateService = matchTemplateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();

            // Ensure the user is an organizer or admin
            if (!IsUserOrganizer() && !IsUserAdmin())
            {
                return Forbid();
            }

            // Both admins and organizers should only see their own templates
            var templates = await _matchTemplateService.GetAllByUserIdAsync(userId);
            return Ok(templates);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();

            // Ensure the user is an organizer or admin
            if (!IsUserOrganizer() && !IsUserAdmin())
            {
                return Forbid();
            }

            // Both admins and organizers should only access their own templates
            var template = await _matchTemplateService.GetByIdAsync(id, userId);
            if (template == null)
                return NotFound();

            return Ok(template);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMatchTemplateDto dto)
        {
            var userId = GetUserId();

            // Ensure the user is an organizer or admin
            if (!IsUserOrganizer() && !IsUserAdmin())
            {
                return Forbid();
            }

            var created = await _matchTemplateService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMatchTemplateDto dto)
        {
            var userId = GetUserId();

            // Ensure the user is an organizer or admin
            if (!IsUserOrganizer() && !IsUserAdmin())
            {
                return Forbid();
            }

            // Both admins and organizers should only update their own templates
            var updated = await _matchTemplateService.UpdateAsync(id, dto, userId);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();

            // Ensure the user is an organizer or admin
            if (!IsUserOrganizer() && !IsUserAdmin())
            {
                return Forbid();
            }

            // Both admins and organizers should only delete their own templates
            var result = await _matchTemplateService.DeleteAsync(id, userId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        private bool IsUserOrganizer()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            return role == UserRole.ORGANISER.ToString();
        }
        private bool IsUserAdmin()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            return role == UserRole.ADMIN.ToString();
        }
    }
}
