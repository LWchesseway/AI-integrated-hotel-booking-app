using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.User;

public class PermissionDto
{
    public int Id { get; set; }
    public string PermissionKey { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Module { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreatePermissionDto
{
    [Required]
    [MaxLength(100)]
    public string PermissionKey { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Module { get; set; }
}

public class UpdatePermissionDto
{
    [Required]
    [MaxLength(100)]
    public string PermissionKey { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Module { get; set; }
}

public class PermissionModuleDto
{
    public string Module { get; set; } = string.Empty;
    public IEnumerable<PermissionDto> Permissions { get; set; } = Enumerable.Empty<PermissionDto>();
}
