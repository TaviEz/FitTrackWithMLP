using FitTrackWithMLP.Shared.DTOs.DailyPlan;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Create;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Get;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Update;
using FitTrackWithMLP.Shared.Enums;

namespace DailyPlanService.Services.DailyPlan
{
    public interface IDailyPlanService
    {
        Task<DailyPlanDto?> GetDailyPlanAsync(string userId, DateOnly targetDate);
        Task<CreateDailyPlanStatus> CreateDailyPlanAsync(
            string userId, DateOnly targetDate, CreateDailyPlanDto dailyPlanDto);
        Task<ReplaceDailyPlanStatus> ReplaceDailyPlanAsync(
            string userId, int dailyPlanId, CreateDailyPlanDto dailyPlanDto);
        Task<UpdateMealPlanStatus> UpdatePlannedMealAsync(
            string userId, int plannedMealId, UpdatePlannedMealDto updateDto);
        Task<DeletePlannedIngredientStatus> DeletePlannedIngredientAsync(
            string userId, int plannedMealId, int plannedIngredientId);
    }
}
