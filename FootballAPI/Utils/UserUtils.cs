using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FootballAPI.Utils
{
    public static class UserUtils
    {
        public static int GetCurrentUserId(ClaimsPrincipal user, IHeaderDictionary headers)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                if (headers.TryGetValue("UserId", out var userIdHeader))
                {
                    if (int.TryParse(userIdHeader.FirstOrDefault(), out var userId))
                        return userId;
                }

                throw new UnauthorizedAccessException("User ID not found in token or header");
            }

            if (int.TryParse(userIdClaim, out var userIdFromClaim))
                return userIdFromClaim;

            throw new UnauthorizedAccessException("Invalid user ID in token");
        }
    }
}
