using Microsoft.AspNetCore.Identity;
using UserManagementService.Models;

namespace UserManagementService.Services.Authentication
{
    public interface IIdentityService
    {
        Task<ApplicationUser?> AuthenticateAsync(string email, string password);
        Task<IdentityResult> RegisterAsync(string email, string password);
        Task<ApplicationUser?> FindByIdAsync(string userId);
    }
}
