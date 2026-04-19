using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.User;
using DoAn.HotelParking.Core.Domain.Entities.Auth;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class PermissionMappingProfile : Profile
{
    public PermissionMappingProfile()
    {
        CreateMap<Permission, PermissionDto>()
            .ForMember(dest => dest.PermissionKey, opt => opt.MapFrom(src => src.PermissionKey ?? string.Empty));

        CreateMap<CreatePermissionDto, Permission>();
        CreateMap<UpdatePermissionDto, Permission>();
    }
}
