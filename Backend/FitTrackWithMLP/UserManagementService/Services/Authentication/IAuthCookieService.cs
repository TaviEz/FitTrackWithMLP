namespace UserManagementService.Services.Authentication
{
    public interface IAuthCookieService
    {
        void AppendAuthCookies(HttpContext httpContext, string accessToken);
        void DeleteAuthCookies(HttpContext httpContext);
    }
}
