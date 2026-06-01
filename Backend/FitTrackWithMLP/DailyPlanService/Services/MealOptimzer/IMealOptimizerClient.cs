using FitTrackWithMLP.Shared.DTOs.DailyPlan.Generate;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Get;

namespace DailyPlanService.Services.MealOptimzer
{
    public interface IMealOptimizerClient
    {
        Task<List<GeneratedMealPlanDto>?> OptimizeAsync(OptimizedRequestDto request, CancellationToken cancellationToken = default);
        Task<List<IngredientOptionDto>> GetIngredientOptionsAsync( string query);
    }
}
