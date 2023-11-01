using System.Security.Claims;
using System.Security.Principal;

namespace Bookstore.Web.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetSub(this IPrincipal claimsPrincipal)
        {
            return ((ClaimsPrincipal)claimsPrincipal).FindFirst(x => x.Type.Contains("nameidentifier"))?.Value;
        }

        public static string GetSub(this ClaimsIdentity identity)
        {
            return identity.FindFirst(x => x.Type.Contains("nameidentifier"))?.Value;
        }
    }
}