using FitTrack.Models;

namespace FitTrack.DTOs
{
    public class UserDetailsDto
    {
        public string Id { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        public string ActivityLevel { get; set; }
    }
}
