using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Bookstore.Domain.Customers;
using Microsoft.AspNetCore.Owin;


namespace Bookstore.Web.Helpers
{
    public class LocalAuthenticationMiddleware : OwinMiddleware
    {
        private const string UserId = "FB6135C7-1464-4A72-B74E-4B63D343DD09";

        private readonly ICustomerService _customerService;

        public LocalAuthenticationMiddleware(OwinMiddleware next, ICustomerService customerService) : base(next)
        {
            _customerService = customerService;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Path.Value.StartsWith("/Authentication/Login"))
            {
                CreateClaimsPrincipal(context);

                await SaveCustomerDetailsAsync();

                var userCookie = new HttpCookie("LocalAuthentication") { Expires = DateTime.Now.AddDays(1) };

                HttpContext.Current.Response.Cookies.Add(userCookie);

                context.Response.Redirect("/");
            }
            else if (HttpContext.Current.Request.Cookies["LocalAuthentication"] != null)
            {
                CreateClaimsPrincipal(context);

                await SaveCustomerDetailsAsync();

                await Next.Invoke(context);
            }
            else
            {
                await Next.Invoke(context);
            }
        }

        private void CreateClaimsPrincipal(IOwinContext context)
        {
            var identity = new ClaimsIdentity("Application");

            identity.AddClaim(new Claim(ClaimTypes.Name, "bookstoreuser"));
            identity.AddClaim(new Claim("nameidentifier", UserId));
            identity.AddClaim(new Claim("given_name", "Bookstore"));
            identity.AddClaim(new Claim("family_name", "User"));
            identity.AddClaim(new Claim(ClaimTypes.Role, "Administrators"));

            context.Request.User = new ClaimsPrincipal(identity);
        }

        private async Task SaveCustomerDetailsAsync()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;

            var dto = new CreateOrUpdateCustomerDto(
                identity.FindFirst("nameidentifier").Value,
                identity.Name,
                identity.FindFirst("given_name").Value,
                identity.FindFirst("family_name").Value);

            await _customerService.CreateOrUpdateCustomerAsync(dto);
        }
    }
}