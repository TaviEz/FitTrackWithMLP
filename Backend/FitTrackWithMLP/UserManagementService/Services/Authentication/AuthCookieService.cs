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

        public void AppendAuthCookies(HttpContext httpContext, string accessToken, IWebHostEnvironment env)
        {
            bool isDev = env.IsDevelopment();

            // 1. Add the access token in the cookie
            httpContext.Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = !isDev, // False for local container testing, True for Azure Production HTTPS
                SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.None, // Lax for local ports, None for true cloud origins
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            // 2. Add the XSRF token in the cookie
            var xsrfTokens = _antiforgery.GetAndStoreTokens(httpContext);
            httpContext.Response.Cookies.Append("XSRF-TOKEN", xsrfTokens.RequestToken!, new CookieOptions
            {
                HttpOnly = false,
                Secure = !isDev,  // False for local container testing, True for Azure Production HTTPS
                SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.None
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
