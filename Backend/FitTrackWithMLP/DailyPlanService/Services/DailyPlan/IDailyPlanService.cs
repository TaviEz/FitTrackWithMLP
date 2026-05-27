using FitTrackWithMLP.Shared.DTOs.DailyPlan;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Create;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Edit;
using FitTrackWithMLP.Shared.Enums;

namespace DailyPlanService.Services.DailyPlan
{
    public interface IDailyPlanService
    {
        Task<DailyPlanDto?> GetDailyPlanAsync(string userId, DateOnly dateTarget);
        Task<CreateDailyPlanStatus> CreateDailyPlanAsync(string userId, CreateDailyPlanDto dailyPlanDto);
        Task<bool> ReplaceDailyPlanAsync(string userId, int dailyPlanId, EditDailyPlanDto dailyPlanDto);
    }
}
