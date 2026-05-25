using FitTrackWithMLP.Shared.DTOs.DailyPlan;
using FitTrackWithMLP.Shared.Enums;

namespace DailyPlanService.Services.DailyPlan
{
    public interface IDailyPlanService
    {
        Task<DailyPlanDto?> GetDailyPlanAsync(string userId, DateOnly dateTarget);
        Task<CreateDailyPlanStatus> CreateDailyPlanAsync(string userId, CreateDailyPlanDto dailyPlanDto);
    }
}
