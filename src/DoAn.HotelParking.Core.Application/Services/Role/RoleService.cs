using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Role;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Role;
using DoAn.HotelParking.Core.Application.Services.Base;

namespace DoAn.HotelParking.Core.Application.Services.Role;

public class RoleService : CrudService<DoAn.HotelParking.Core.Domain.Entities.Auth.Role, RoleDto, CreateRoleDto, UpdateRoleDto>, IRoleService
{
    public RoleService(
        IRoleRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(repository, unitOfWork, mapper)
    {
    }
}