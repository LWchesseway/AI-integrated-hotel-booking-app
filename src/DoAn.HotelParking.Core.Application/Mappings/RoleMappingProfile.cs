using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Role;
using DoAn.HotelParking.Core.Domain.Entities.Auth;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class RoleMappingProfile : Profile
{
    public RoleMappingProfile()
    {
        CreateMap<Role, RoleDto>();
        CreateMap<CreateRoleDto, Role>();
        CreateMap<UpdateRoleDto, Role>();
    }
}