using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Auth;

namespace DoAn.HotelParking.Core.Application.Interfaces.User;

public interface IPermissionRepository : IGenericRepository<Permission>
{
    Task<Permission?> GetByKeyAsync(string permissionKey, CancellationToken cancellationToken = default);
    Task<bool> PermissionKeyExistsAsync(string permissionKey, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetByModuleAsync(string module, CancellationToken cancellationToken = default);
    Task<IEnumerable<IGrouping<string, Permission>>> GetAllGroupedByModuleAsync(CancellationToken cancellationToken = default);

    Task<bool> UserHasPermissionAsync(int userId, string permissionKey, CancellationToken cancellationToken = default);
    Task<List<string>> GetUserPermissionKeysAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<string>> GetUserRoleNamesAsync(int userId, CancellationToken cancellationToken = default);
}
