using UserManagementService.Models;

namespace UserManagementService.Services.Authentication
{
    public interface ITokenService
    {
        string CreateAccessToken(ApplicationUser user);
    }
}
