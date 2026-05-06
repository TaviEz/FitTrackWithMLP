using System.Text.Json.Serialization;

namespace FitTrackWithMLP.Shared.DTOs
{
    public class OptimizedRequestDto
    {
        [JsonPropertyName("calories")]
        public int Calories { get; set; }
        [JsonPropertyName("protein")]
        public int Protein { get; set; }
        [JsonPropertyName("min_fat")]
        public int MinFat { get; set; }
        [JsonPropertyName("meals_complexity")]
        public string MealsComplexity { get; set; } = string.Empty;
    }
}
