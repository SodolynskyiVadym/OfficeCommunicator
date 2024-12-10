using AutoMapper;
using OfficeCommunicatorAPI.DTO;
using OfficeCommunicatorAPI.Models;

namespace OfficeCommunicatorAPI.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // CreateMap<User, UserUpdateDto>();

            CreateMap<User, UserDto>();
            
            CreateMap<(UserDto, byte[][] passwordData), User>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Item1.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Item1.Email))
                .ForMember(dest => dest.UniqueName, opt => opt.MapFrom(src => src.Item1.UniqueName))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.passwordData[0]))
                .ForMember(dest => dest.PasswordSalt, opt => opt.MapFrom(src => src.passwordData[1]));

            CreateMap<GroupDto, Group>();
            
            CreateMap<ContactDto, Contact>();
        }
    }
}
