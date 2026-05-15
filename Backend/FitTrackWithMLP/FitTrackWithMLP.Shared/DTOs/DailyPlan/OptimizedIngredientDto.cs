using System.Text.Json.Serialization;

namespace FitTrackWithMLP.Shared.DTOs.DailyPlan
{
    public class OptimizedIngredientDto
    {
        [JsonPropertyName("foodId")]
        public int FoodId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("amountG")]
        public int AmountG { get; set; }

        [JsonPropertyName("calories")]
        public int Calories { get; set; }

        [JsonPropertyName("protein")]
        public double Protein { get; set; }

        [JsonPropertyName("fats")]
        public double Fats { get; set; }

        [JsonPropertyName("carbs")]
        public double Carbs { get; set; }
    }
}
