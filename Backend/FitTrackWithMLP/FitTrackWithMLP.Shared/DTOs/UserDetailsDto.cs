namespace FitTrackWithMLP.Shared.DTOs
{
    public class UserDetailsDto
    {
        public string Id { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        public string ActivityLevel { get; set; }
        public int Bmr { get; set; }
        public int Tdee { get; set; }
    }
}
