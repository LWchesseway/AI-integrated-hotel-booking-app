using DoAn.HotelParking.Core.Application.DTOs.User;

namespace DoAn.HotelParking.Core.Application.Interfaces.User;

public interface IPermissionManagementService
{
    Task<(IEnumerable<PermissionDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        string? keyword = null,
        string? module = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<PermissionDto>> GetByModuleAsync(string module, CancellationToken cancellationToken = default);
    Task<IEnumerable<PermissionModuleDto>> GetGroupedByModuleAsync(CancellationToken cancellationToken = default);

    Task<PermissionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PermissionDto> CreateAsync(CreatePermissionDto dto, CancellationToken cancellationToken = default);
    Task<PermissionDto?> UpdateAsync(int id, UpdatePermissionDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
