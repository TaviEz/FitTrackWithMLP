using FitTrackWithMLP.Shared.Enums;

namespace UserManagementService.Models
{
    public class UserDetails
    {
        public int Id { get; set; }
        public Gender Gender { get; set; }
        public int Age { get; set; }
        public int Weight { get; set; } // in kg
        public int Height { get; set; } // in cm
        public ActivityLevel ActivityLevel { get; set; }
        public DateTime Date { get; set; }
        public int Bmr { get; set; }
        public int Tdee { get; set; }
        public GoalType GoalType { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
