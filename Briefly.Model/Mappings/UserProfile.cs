using AutoMapper;
using Briefly.Model.DTOs;
using Briefly.Model.Entities;

namespace Briefly.Model.Mappings
{
    // Fix: Inherit from AutoMapper.Profile and move CreateMap calls to the constructor
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // DTO → Entity (for Registration)
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Password will be hashed manually

            // Entity → DTO (for API Response)
            CreateMap<User, UserResponseDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role!.RoleName));
        }
    }
}
