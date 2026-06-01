using FitTrackWithMLP.Shared.Enums.Lookups;

namespace FitTrackWithMLP.Shared.DTOs.User
{
    public class UserPhysiqueDto
    {
        public int Tdee { get; set; }
        public int Weight { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
        public GoalType GoalType { get; set; }
        public MealsComplexity MealsComplexity { get; set; }
    }
}
