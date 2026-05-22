using DoAn.HotelParking.Core.Application.DTOs.Role;

namespace DoAn.HotelParking.Core.Application.Interfaces.Role;

public interface IRoleService
{
	Task<IEnumerable<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<(IEnumerable<RoleDto> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
	Task<RoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<RoleDto> CreateAsync(CreateRoleDto dto, CancellationToken cancellationToken = default);
	Task<RoleDto?> UpdateAsync(int id, UpdateRoleDto dto, CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}