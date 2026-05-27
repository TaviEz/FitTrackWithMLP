namespace FitTrackWithMLP.Shared.DTOs.DailyPlan.Create
{
    public class CreatePlannedIngredientDto
    {
        public int FoodId { get; set; } // SQLite ID
        public int AmountG { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Protein { get; set; }
        public double Fats { get; set; }
        public double Carbs { get; set; }
        public int Calories { get; set; }
    }
}
