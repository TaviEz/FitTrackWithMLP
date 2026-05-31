namespace DailyPlanService.Models
{
    public class PlannedIngredient
    {
        public int PlannedIngredientId { get; set; }
        public int FoodId { get; set; }     // ID reference to SQLite
        public int AmountG { get; set; }    // Calculated weight

        // Snapshotted Values (Stored at the time of generation)
        public string Name { get; set; } = string.Empty;
        // Nutritional values per 100g
        public double Protein { get; set; }
        public double Fats { get; set; }
        public double Carbs { get; set; }
        public int Calories { get; set; }

        // Foreign key to PlannedMeal
        public int PlannedMealId { get; set; }
        public PlannedMeal PlannedMeal { get; set; } = null!;
    }
}
