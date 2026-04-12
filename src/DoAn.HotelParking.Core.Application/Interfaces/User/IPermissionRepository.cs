using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Auth;
namespace DoAn.HotelParking.Core.Application.Interfaces.User;

public interface IPermissionRepository : IGenericRepository<Permission>
{
    Task<bool> UserHasPermissionAsync(int userId, string permissionKey);
    Task<IEnumerable<string>> GetUserPermissionKeysAsync(int userId);
    Task<IEnumerable<string>> GetUserRoleNamesAsync(int userId);
    Task<Permission?> GetByKeyAsync(string permissionKey);
    Task<bool> KeyExistsAsync(string permissionKey, int? excludeId = null);
    Task<IEnumerable<Permission>> GetByModuleAsync(string module);
    Task<Dictionary<string, List<Permission>>> GetAllGroupedByModuleAsync();
}
