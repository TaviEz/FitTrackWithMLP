namespace FitTrackWithMLP.Shared.DTOs
{
    public class UserPhysiqueDto
    {
        public int Tdee { get; set; }
        public int Weight { get; set; }
        public string ActivityLevel { get; set; } = string.Empty;
        public string GoalType { get; set; } = string.Empty;
        public string MealsComplexity { get; set; } = "Standard";
    }
}
