namespace DoAn.HotelParking.Core.Domain.Entities.Auth;

public class Permission
{
    public int Id { get; set; }
    public string? PermissionKey { get; set; }
    public string? Description { get; set; }
    public string? Module { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();
}