using FitTrackWithMLP.Shared.DTOs.DailyPlan.Create;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Edit;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Get;
using FitTrackWithMLP.Shared.Enums;

namespace DailyPlanService.Services.DailyPlan
{
    public interface IDailyPlanService
    {
        Task<DailyPlanDto?> GetDailyPlanAsync(string userId, DateOnly targetDate);
        Task<CreateDailyPlanStatus> CreateDailyPlanAsync(string userId, DateOnly targetDate, CreateDailyPlanDto dailyPlanDto);
        Task<ReplaceDailyPlanStatus> ReplaceDailyPlanAsync(string userId, int dailyPlanId, EditDailyPlanDto dailyPlanDto);
    }
}
