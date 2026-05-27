namespace FitTrackWithMLP.Shared.DTOs.DailyPlan.Edit
{
    public class EditPlannedIngredientDto
    {
        public int FoodId { get; set; }
        public int AmountG { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Protein { get; set; }
        public double Fats { get; set; }
        public double Carbs { get; set; }
        public int Calories { get; set; }
    }
}
