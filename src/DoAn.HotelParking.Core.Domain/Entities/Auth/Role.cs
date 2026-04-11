namespace DoAn.HotelParking.Core.Domain.Entities.Auth;

public class Role
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();
}