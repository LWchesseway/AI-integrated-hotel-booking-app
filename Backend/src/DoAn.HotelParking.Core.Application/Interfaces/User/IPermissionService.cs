namespace DoAn.HotelParking.Core.Application.Interfaces.User;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(int userId, string permissionKey, CancellationToken cancellationToken = default);
    Task<List<string>> GetUserPermissionKeysAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<string>> GetUserRoleNamesAsync(int userId, CancellationToken cancellationToken = default);
}
