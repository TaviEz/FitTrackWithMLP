using Microsoft.AspNetCore.Antiforgery;

namespace UserManagementService.Services.Authentication
{
    public class AuthCookieService: IAuthCookieService
    {
        private readonly IAntiforgery _antiforgery;

        public AuthCookieService(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void AppendAuthCookies(HttpContext httpContext, string accessToken)
        {
            // add the token in the cookie
            httpContext.Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // use HTTPS
                SameSite = SameSiteMode.None, // needed for frontend on different origin
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            // add the XSRF token in the cookie
            var xsrfTokens = _antiforgery.GetAndStoreTokens(httpContext);
            httpContext.Response.Cookies.Append("XSRF-TOKEN", xsrfTokens.RequestToken!, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None
            });
        }

        public void DeleteAuthCookies(HttpContext httpContext)
        {
            httpContext.Response.Cookies.Delete("access_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
        }
    }
}
