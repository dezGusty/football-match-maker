using Microsoft.AspNetCore.Mvc;
using FootballAPI.Service.Interfaces;
using FootballAPI.DTOs;

namespace FootballAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingHistoryController : ControllerBase
    {
        private readonly IRatingHistoryService _ratingHistoryService;

        public RatingHistoryController(IRatingHistoryService ratingHistoryService)
        {
            _ratingHistoryService = ratingHistoryService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<RatingHistoryDto>>> GetUserRatingHistory(
            int userId,
            [FromQuery] GetRatingHistoryDto? filters = null)
        {
            try
            {
                var ratingHistory = await _ratingHistoryService.GetUserRatingHistoryAsync(userId, filters);
                return Ok(ratingHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("match/{matchId}")]
        public async Task<ActionResult<IEnumerable<RatingHistoryDto>>> GetMatchRatingChanges(int matchId)
        {
            try
            {
                var ratingChanges = await _ratingHistoryService.GetMatchRatingChangesAsync(matchId);
                return Ok(ratingChanges);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/trend")]
        public async Task<ActionResult<RatingTrendDto>> GetUserRatingTrend(
            int userId,
            [FromQuery] int? lastNMatches = null)
        {
            try
            {
                var ratingTrend = await _ratingHistoryService.GetUserRatingTrendAsync(userId, lastNMatches);
                return Ok(ratingTrend);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/statistics")]
        public async Task<ActionResult<RatingStatisticsDto>> GetUserRatingStatistics(int userId)
        {
            try
            {
                var statistics = await _ratingHistoryService.GetUserRatingStatisticsAsync(userId);
                return Ok(statistics);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/recent")]
        public async Task<ActionResult<IEnumerable<RatingHistoryDto>>> GetRecentRatingChanges(
            int userId,
            [FromQuery] int count = 10)
        {
            try
            {
                var recentChanges = await _ratingHistoryService.GetRecentRatingChangesAsync(userId, count);
                return Ok(recentChanges);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/rating-at-date")]
        public async Task<ActionResult<float?>> GetUserRatingAtDate(
            int userId,
            [FromQuery] DateTime date)
        {
            try
            {
                var rating = await _ratingHistoryService.GetUserRatingAtDateAsync(userId, date);
                if (rating.HasValue)
                    return Ok(new { rating = rating.Value, date = date });
                else
                    return NotFound(new { message = "No rating data found for the specified date" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("user/{userId}/by-reason/{changeReason}")]
        public async Task<ActionResult<IEnumerable<RatingHistoryDto>>> GetRatingChangesByReason(
            int userId,
            string changeReason)
        {
            try
            {
                var ratingChanges = await _ratingHistoryService.GetRatingChangesByReasonAsync(userId, changeReason);
                return Ok(ratingChanges);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<RatingHistoryDto>> CreateRatingHistory([FromBody] CreateRatingHistoryDto dto)
        {
            try
            {
                var ratingHistory = await _ratingHistoryService.CreateRatingHistoryAsync(dto);
                return CreatedAtAction(nameof(GetUserRatingHistory),
                    new { userId = dto.UserId }, ratingHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRatingHistory(int id)
        {
            try
            {
                var success = await _ratingHistoryService.DeleteRatingHistoryAsync(id);
                if (success)
                    return Ok(new { message = "Rating history entry deleted successfully" });
                else
                    return NotFound(new { message = "Rating history entry not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}