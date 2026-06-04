namespace UserManagementService.Services.Authentication
{
    public interface IAuthCookieService
    {
        void AppendAuthCookies(HttpContext httpContext, string accessToken, IWebHostEnvironment env);
        void DeleteAuthCookies(HttpContext httpContext);
    }
}
