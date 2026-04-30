namespace FitTrack.Models
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

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }

    public enum ActivityLevel
    {
        Sedentary,
        LightlyActive,
        ModeratelyActive,
        VeryActive,
        ExtraActive
    }
}
