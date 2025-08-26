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
                throw new UnauthorizedAccessException("User ID not found in token");
            }

            if (int.TryParse(userIdClaim, out var userIdFromClaim))
                return userIdFromClaim;

            throw new UnauthorizedAccessException("Invalid user ID in token");
        }
    }
}

