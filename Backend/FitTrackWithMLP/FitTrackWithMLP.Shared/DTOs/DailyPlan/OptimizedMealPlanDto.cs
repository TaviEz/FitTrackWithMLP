using System.Text.Json.Serialization;

namespace FitTrackWithMLP.Shared.DTOs.DailyPlan
{
    public class OptimizedMealPlanDto
    {
        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("meal_id")]
        public int MealId { get; set; }

        [JsonPropertyName("ingredients")]
        public List<OptimizedIngredientDto> Ingredients { get; set; } = new();

        [JsonPropertyName("error")]
        public double Error { get; set; }

        [JsonPropertyName("calories")]
        public int Calories { get; set; }
    }
}
