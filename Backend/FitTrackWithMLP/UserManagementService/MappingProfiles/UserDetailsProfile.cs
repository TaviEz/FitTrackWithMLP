using AutoMapper;
using FitTrackWithMLP.Shared.DTOs.User;
using UserManagementService.Models;

namespace UserManagementService.MappingProfiles
{
    public class UserDetailsProfile: Profile
    {
        public UserDetailsProfile()
        {
            CreateMap<UserDetailsDto, UserDetails>()
                .ForMember(dest => dest.TargetCalories, opt => opt.Ignore());

            CreateMap<UserDetails, UserDetailsDto>();
        }
    }
}
