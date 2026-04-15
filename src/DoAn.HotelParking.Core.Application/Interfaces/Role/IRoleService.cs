using DoAn.HotelParking.Core.Application.DTOs.Role;
using DoAn.HotelParking.Core.Application.Interfaces.Base;

namespace DoAn.HotelParking.Core.Application.Interfaces.Role;

public interface IRoleService : ICrudService<RoleDto, CreateRoleDto, UpdateRoleDto>
{
}