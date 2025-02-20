using System;
using System.Drawing;
using System.Net;
using Microsoft.AspNetCore.Owin;

using Microsoft.AspNetCore.Http;


namespace Bookstore.Web.Helpers
{
    public static class HttpContextExtensions
    {
        public static string GetShoppingCartCorrelationId(this HttpContext context)
        {
            var CookieKey = "ShoppingCartId";

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddYears(1),
                Path = "/"
            };

            HttpCookie cookie = context.Request.Cookies[CookieKey];
            string shoppingCartClientId = cookie != null ? cookie.Value : null;

            //var shoppingCartClientId = context.Request.Cookies[CookieKey].Value;

            if (string.IsNullOrWhiteSpace(shoppingCartClientId))
            {
                shoppingCartClientId = context.User.Identity.IsAuthenticated ? context.User.GetSub() : Guid.NewGuid().ToString();
            }

            context.Response.Cookies.Add(new HttpCookie(CookieKey, shoppingCartClientId));

            return shoppingCartClientId;
        }
    }
}