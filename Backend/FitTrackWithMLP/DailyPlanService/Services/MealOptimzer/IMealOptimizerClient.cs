using FitTrackWithMLP.Shared.DTOs.DailyPlan;

namespace DailyPlanService.Services.MealOptimzer
{
    public interface IMealOptimizerClient
    {
        Task<List<OptimizedMealPlanDto>?> OptimizeAsync(OptimizedRequestDto request, CancellationToken cancellationToken = default);
    }
}
