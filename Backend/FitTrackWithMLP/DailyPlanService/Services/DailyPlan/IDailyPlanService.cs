using FitTrackWithMLP.Shared.DTOs.DailyPlan.Create;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Get;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Update;
using FitTrackWithMLP.Shared.Enums.Statuses;

namespace DailyPlanService.Services.DailyPlan
{
    public interface IDailyPlanService
    {
        Task<DailyPlanDto?> GetDailyPlanAsync(string userId, DateOnly targetDate);
        Task<CreateDailyPlanStatus> CreateDailyPlanAsync(
            string userId, DateOnly targetDate, CreateDailyPlanDto dailyPlanDto);
        Task<ReplaceDailyPlanStatus> ReplaceDailyPlanAsync(
            string userId, int dailyPlanId, CreateDailyPlanDto dailyPlanDto);
        Task<List<int>> GetLatestPlannedMealsAsync(
            string userId, int daysAgo = 7, int mealCap = 10);
        Task<AddPlannedMealResult> AddPlannedMealAsync(
            string userId, int dailyPlanId, AddPlannedMealItemDto addDto);
        Task<UpdatePlannedMealTitleStatus> UpdatePlannedMealTitleAsync(
            string userId, int plannedMealId, string title);
        Task<AddPlannedIngredientStatus> AddPlannedIngredientAsnyc(
            string userId, int plannedMealId, CreatePlannedIngredientDto addDto);
        Task<UpdatePlannedIngredientStatus> UpdatePlannedIngredientAsync(
            string userId, int plannedIngredientId, UpdatePlannedIngredientDto updateDto);
        Task<DeletePlannedIngredientStatus> DeletePlannedIngredientAsync(
            string userId, int plannedMealId, int plannedIngredientId);
    }
}
