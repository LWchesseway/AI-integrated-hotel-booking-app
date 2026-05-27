using DoAn.HotelParking.Core.Application.Interfaces.User;

namespace DoAn.HotelParking.Core.Application.Services.User;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;

    public PermissionService(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public Task<bool> HasPermissionAsync(int userId, string permissionKey, CancellationToken cancellationToken = default)
    {
        if (userId <= 0 || string.IsNullOrWhiteSpace(permissionKey))
        {
            return Task.FromResult(false);
        }

        return _permissionRepository.UserHasPermissionAsync(userId, permissionKey, cancellationToken);
    }

    public Task<List<string>> GetUserPermissionKeysAsync(int userId, CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            return Task.FromResult(new List<string>());
        }

        return _permissionRepository.GetUserPermissionKeysAsync(userId, cancellationToken);
    }

    public Task<List<string>> GetUserRoleNamesAsync(int userId, CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            return Task.FromResult(new List<string>());
        }

        return _permissionRepository.GetUserRoleNamesAsync(userId, cancellationToken);
    }
}
