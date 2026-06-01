using FitTrackWithMLP.Shared.Enums.Statuses;

namespace UserManagementService.Services.UserDetails
{
    public sealed record UserDetailsOperationResult(
    UserDetailsOperationStatus Status,
    string? ErrorMessage = null)
    {
        public static UserDetailsOperationResult Success() =>
            new(UserDetailsOperationStatus.Success);

        public static UserDetailsOperationResult UserNotFound() =>
            new(UserDetailsOperationStatus.UserNotFound, "User not found");

        public static UserDetailsOperationResult Failed(string message) =>
            new(UserDetailsOperationStatus.Failed, message);
    }
}
