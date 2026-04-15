using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.User;
using DoAn.HotelParking.Core.Domain.Entities.Auth;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (byte)src.Status));

        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => UserStatus.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (UserStatus)src.Status));
    }
}