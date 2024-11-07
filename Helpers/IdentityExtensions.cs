using System.Security.Claims;
using System.Security.Principal;

namespace ThienAnFuni.Helpers
{
    public static class IdentityExtensions
    {
        // Extension method để lấy UserId từ ClaimsPrincipal
        public static string GetUserId(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    return userIdClaim.Value;
                }
            }
            return null; // Trả về null nếu không tìm thấy UserId
        }
    }
}
