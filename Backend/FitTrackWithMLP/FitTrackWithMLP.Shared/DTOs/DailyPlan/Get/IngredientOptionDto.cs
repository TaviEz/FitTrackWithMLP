using System.Text.Json.Serialization;

namespace FitTrackWithMLP.Shared.DTOs.DailyPlan.Get
{
    public class IngredientOptionDto
    {
        [JsonPropertyName("food_id")]
        public int FoodId { get; set; } // SQLite auto-incremented primary key
        public string Name { get; set; } = string.Empty;
        public float Protein { get; set; }
        [JsonPropertyName("fat")]
        public float Fats { get; set; }
        public float Carbs { get; set; }
        public int Calories { get; set; }
    }
}
