using DailyPlanService.Services.MealOptimzer.Exceptions;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Generate;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Get;

namespace DailyPlanService.Services.MealOptimzer
{
    public class MealOptimizerClient: IMealOptimizerClient
    {
        private readonly HttpClient _httpClient;

        public MealOptimizerClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<GeneratedMealPlanDto>?> OptimizeAsync(
            OptimizedRequestDto request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/optimize", request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
                    {
                        throw new InfeasibleConstraintsException("The configuration parameters contain mathematically infeasible macro-nutrient boundaries.");
                    }

                    return null;
                }

                return await response.Content.ReadFromJsonAsync<List<GeneratedMealPlanDto>>(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw; // caller canceled: propagate
            }
            catch (InfeasibleConstraintsException)
            {
                throw;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<IngredientOptionDto>> GetIngredientOptionsAsync(string query)
        {
            var response = await _httpClient.GetAsync($"/ingredients?query={query}");
            
            if (!response.IsSuccessStatusCode)
            {
                return new List<IngredientOptionDto>();
            }

            return await response.Content.ReadFromJsonAsync<List<IngredientOptionDto>>() ?? new List<IngredientOptionDto>();
        }
    }
}
