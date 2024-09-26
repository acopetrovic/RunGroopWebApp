using System.Security.Claims;

namespace RunGroopWebApp
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetuserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
