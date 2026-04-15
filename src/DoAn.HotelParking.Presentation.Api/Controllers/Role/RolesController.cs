using DoAn.HotelParking.Core.Application.DTOs.Role;
using DoAn.HotelParking.Core.Application.Interfaces.Role;
using DoAn.HotelParking.Presentation.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Role;

[Route("api/roles")]
[Authorize(Roles = "Admin")]
public class RolesController : CrudControllerBase<RoleDto, CreateRoleDto, UpdateRoleDto>
{
    public RolesController(IRoleService service) : base(service)
    {
    }
}