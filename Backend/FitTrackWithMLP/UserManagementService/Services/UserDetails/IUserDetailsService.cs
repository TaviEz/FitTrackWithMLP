using FitTrackWithMLP.Shared.DTOs.User;
using FitTrackWithMLP.Shared.Enums.Statuses;

namespace UserManagementService.Services.UserDetails
{
    public interface IUserDetailsService
    {
        Task<UserDetailsDto?> GetUserDetailsAsync(string userId);
        Task<UserDetailsOperationResult> UpsertUserDetailsAsync(string userId, UserDetailsDto userDto);
    }
}
