using Microsoft.Owin;

namespace Bookstore.Web.Helpers
{
    public static class OwinRequestExtensions
    {
        public static string GetReturnUrl(this IOwinRequest request)
        {
            return $"{request.Scheme}://{request.Host}/signin-oidc";
        }
    }
}