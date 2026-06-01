using FitTrackWithMLP.Shared.Enums.Lookups;

namespace FitTrackWithMLP.Shared.DTOs.User
{
    public class UserDetailsDto
    {
        public Gender Gender { get; set; }
        public int Age { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
        public int Bmr { get; set; }
        public int Tdee { get; set; }
        public GoalType Goal { get; set; }
        public int TargetCalories { get; set; }
    }
}
