using DoAn.HotelParking.Core.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Auth;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> entity)
    {
        entity.ToTable("RolePermission");

        entity.HasKey(e => new { e.RoleId, e.PermissionId });
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasOne(e => e.Role)
            .WithMany(e => e.RolePermissions)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.Permission)
            .WithMany(e => e.RolePermissions)
            .HasForeignKey(e => e.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}