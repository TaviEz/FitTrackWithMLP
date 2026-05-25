using FitTrackWithMLP.Shared.DTOs.DailyPlan;

namespace DailyPlanService.Services.MealOptimzer
{
    public class MealOptimizerClient: IMealOptimizerClient
    {
        private readonly HttpClient _httpClient;

        public MealOptimizerClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<OptimizedMealPlanDto>?> OptimizeAsync(
            OptimizedRequestDto request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/optimize", request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<List<OptimizedMealPlanDto>>(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw; // caller canceled: propagate
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
