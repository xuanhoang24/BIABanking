using System.Security.Claims;

namespace BankingSystemAPI.Configuration
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Retrieves the authenticated user's identifier claim as an integer.
        /// Supports tokens that emit either "ClaimTypes.NameIdentifier" or "sub".
        /// </summary>
        public static int GetRequiredUserId(this ClaimsPrincipal user)
        {
            var rawId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(rawId))
                throw new InvalidOperationException("User identifier claim is missing.");

            if (!int.TryParse(rawId, out var userId))
                throw new InvalidOperationException("User identifier claim is invalid.");

            return userId;
        }
    }
}
