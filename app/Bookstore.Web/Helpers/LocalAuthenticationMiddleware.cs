using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Bookstore.Domain.Customers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Bookstore.Web.Helpers
{
    public class LocalAuthenticationMiddleware
    {
        private const string UserId = "FB6135C7-1464-4A72-B74E-4B63D343DD09";

        private readonly ICustomerService _customerService;
        private readonly RequestDelegate _next;

        public LocalAuthenticationMiddleware(RequestDelegate next, ICustomerService customerService)
        {
            _next = next;
            _customerService = customerService;
        }

        public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext context)
        {
            if (context.Request.Path.Value.StartsWith("/Authentication/Login"))
            {
                CreateClaimsPrincipal(context);

                await SaveCustomerDetailsAsync();

                // Modern cookie implementation
                context.Response.Cookies.Append("LocalAuthentication", "true", new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1)
                });

                context.Response.Redirect("/");
                return;
            }
            else if (context.Request.Cookies.ContainsKey("LocalAuthentication"))
            {
                CreateClaimsPrincipal(context);

                await SaveCustomerDetailsAsync();

                await _next(context);
            }
            else
            {
                await _next(context);
            }
        }

        private void CreateClaimsPrincipal(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var identity = new ClaimsIdentity("Application");

            identity.AddClaim(new Claim(ClaimTypes.Name, "bookstoreuser"));
            identity.AddClaim(new Claim("nameidentifier", UserId));
            identity.AddClaim(new Claim("given_name", "Bookstore"));
            identity.AddClaim(new Claim("family_name", "User"));
            identity.AddClaim(new Claim(ClaimTypes.Role, "Administrators"));

            context.User = new ClaimsPrincipal(identity);
        }

        private async Task SaveCustomerDetailsAsync()
        {
            // Get the current user from the HttpContext adapter
            var identity = (ClaimsIdentity)System.Web.HttpContext.Current.User.Identity;

            var dto = new CreateOrUpdateCustomerDto(
                identity.FindFirst("nameidentifier").Value,
                identity.Name,
                identity.FindFirst("given_name").Value,
                identity.FindFirst("family_name").Value);

            await _customerService.CreateOrUpdateCustomerAsync(dto);
        }
    }
}