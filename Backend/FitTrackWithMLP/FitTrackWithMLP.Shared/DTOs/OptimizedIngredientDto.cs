using System.Text.Json.Serialization;

namespace FitTrackWithMLP.Shared.DTOs
{
    public class OptimizedIngredientDto
    {
        [JsonPropertyName("food_id")]
        public int FoodId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("amount_g")]
        public int AmountG { get; set; }
    }
}
