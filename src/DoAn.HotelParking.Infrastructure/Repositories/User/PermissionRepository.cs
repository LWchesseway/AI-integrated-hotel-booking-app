using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Core.Domain.Entities.Auth;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.User;

public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
{
    public PermissionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<Permission?> GetByKeyAsync(string permissionKey, CancellationToken cancellationToken = default)
    {
        var normalizedKey = NormalizePermissionKey(permissionKey);

        return DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PermissionKey != null && p.PermissionKey.ToLower() == normalizedKey, cancellationToken);
    }

    public Task<bool> PermissionKeyExistsAsync(
        string permissionKey,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedKey = NormalizePermissionKey(permissionKey);

        var query = DbSet.Where(p => p.PermissionKey != null && p.PermissionKey.ToLower() == normalizedKey);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetByModuleAsync(string module, CancellationToken cancellationToken = default)
    {
        var normalizedModule = NormalizeModule(module);

        return await DbSet
            .AsNoTracking()
            .Where(p => p.Module != null && p.Module.ToLower() == normalizedModule)
            .OrderBy(p => p.PermissionKey)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<IGrouping<string, Permission>>> GetAllGroupedByModuleAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await DbSet
            .AsNoTracking()
            .OrderBy(p => p.Module)
            .ThenBy(p => p.PermissionKey)
            .ToListAsync(cancellationToken);

        return permissions.GroupBy(p => string.IsNullOrWhiteSpace(p.Module) ? "General" : p.Module!.Trim());
    }

    public Task<bool> UserHasPermissionAsync(int userId, string permissionKey, CancellationToken cancellationToken = default)
    {
        var normalizedKey = NormalizePermissionKey(permissionKey);

        return Context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Join(
                Context.RolePermissions.AsNoTracking(),
                ur => ur.RoleId,
                rp => rp.RoleId,
                (_, rp) => rp.PermissionId)
            .Join(
                Context.Permissions.AsNoTracking(),
                permissionId => permissionId,
                permission => permission.Id,
                (_, permission) => permission.PermissionKey)
            .Where(key => key != null)
            .AnyAsync(key => key!.ToLower() == normalizedKey, cancellationToken);
    }

    public async Task<List<string>> GetUserPermissionKeysAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Join(
                Context.RolePermissions.AsNoTracking(),
                ur => ur.RoleId,
                rp => rp.RoleId,
                (_, rp) => rp.PermissionId)
            .Join(
                Context.Permissions.AsNoTracking(),
                permissionId => permissionId,
                permission => permission.Id,
                (_, permission) => permission.PermissionKey)
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .Select(key => key!)
            .Distinct()
            .OrderBy(key => key)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<string>> GetUserRoleNamesAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Join(
                Context.Roles.AsNoTracking(),
                ur => ur.RoleId,
                role => role.Id,
                (_, role) => role.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .Distinct()
            .OrderBy(name => name)
            .ToListAsync(cancellationToken);
    }

    private static string NormalizePermissionKey(string permissionKey)
    {
        return permissionKey.Trim().ToLowerInvariant();
    }

    private static string NormalizeModule(string module)
    {
        return module.Trim().ToLowerInvariant();
    }
}
