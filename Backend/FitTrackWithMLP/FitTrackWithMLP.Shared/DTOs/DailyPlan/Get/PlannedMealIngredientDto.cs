namespace FitTrackWithMLP.Shared.DTOs.DailyPlan.Get
{
    public class PlannedMealIngredientDto
    {
        public int PlannedIngredientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int AmountG { get; set; }
        public float Protein { get; set; }
        public float Fats { get; set; }
        public float Carbs { get; set; }
        public int Calories { get; set; }
    }
}
