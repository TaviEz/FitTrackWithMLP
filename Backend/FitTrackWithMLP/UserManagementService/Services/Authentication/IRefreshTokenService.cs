using UserManagementService.Models;

namespace UserManagementService.Services.Authentication
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateAndStoreAsync(string userId);
        Task DeleteTokensByUserIdAsync(string userId);
        Task<RefreshToken?> ValidateAsync(string token);
        Task DeleteAsync(string token);
    }
}
