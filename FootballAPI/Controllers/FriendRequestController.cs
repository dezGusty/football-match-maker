using FootballAPI.DTOs;
using FootballAPI.Service;
using FootballAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FootballAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FriendRequestController : ControllerBase
    {
        private readonly IFriendRequestService _friendRequestService;
        private readonly ILogger<FriendRequestController> _logger;

        public FriendRequestController(IFriendRequestService friendRequestService, ILogger<FriendRequestController> logger)
        {
            _friendRequestService = friendRequestService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<FriendRequestDto>> SendFriendRequest([FromBody] CreateFriendRequestDto dto)
        {
            try
            {
                var senderId = UserUtils.GetCurrentUserId(User, Request.Headers);

                var result = await _friendRequestService.SendFriendRequestAsync(senderId, dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending friend request");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/respond")]
        public async Task<ActionResult<FriendRequestDto>> RespondToFriendRequest(int id, [FromBody] FriendRequestResponseDto response)
        {
            try
            {
                var userId = UserUtils.GetCurrentUserId(User, Request.Headers);
                var result = await _friendRequestService.RespondToFriendRequestAsync(id, userId, response);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error responding to friend request");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("sent")]
        public async Task<ActionResult<IEnumerable<FriendRequestDto>>> GetSentRequests()
        {
            try
            {
                var userId = UserUtils.GetCurrentUserId(User, Request.Headers);
                var result = await _friendRequestService.GetSentRequestsAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sent requests");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("received")]
        public async Task<ActionResult<IEnumerable<FriendRequestDto>>> GetReceivedRequests()
        {
            try
            {
                var userId = UserUtils.GetCurrentUserId(User, Request.Headers);
                var result = await _friendRequestService.GetReceivedRequestsAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting received requests");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("friends")]
        public async Task<ActionResult<IEnumerable<FriendRequestDto>>> GetFriends()
        {
            try
            {
                var userId = UserUtils.GetCurrentUserId(User, Request.Headers);
                var result = await _friendRequestService.GetFriendsAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting friends");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFriendRequest(int id)
        {
            try
            {
                var userId = UserUtils.GetCurrentUserId(User, Request.Headers);
                var success = await _friendRequestService.DeleteFriendRequestAsync(id, userId);

                if (!success)
                    return NotFound("Friend request not found");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting friend request");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("unfriend/{friendId}")]
        public async Task<ActionResult> Unfriend(int friendId)
        {
            try 
            {
                var userId = UserUtils.GetCurrentUserId(User, Request.Headers);
                var success = await _friendRequestService.UnfriendAsync(userId, friendId);
                if (!success)
                    return NotFound("Friend relationship not found");
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unfriending user");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
