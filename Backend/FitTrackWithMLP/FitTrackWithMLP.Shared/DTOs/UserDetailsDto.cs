namespace FitTrackWithMLP.Shared.DTOs
{
    public class UserDetailsDto
    {
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        public string ActivityLevel { get; set; } = string.Empty;
        public int Bmr { get; set; }
        public int Tdee { get; set; }
    }
}
