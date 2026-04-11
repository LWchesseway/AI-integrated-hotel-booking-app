using DoAn.HotelParking.Core.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Auth;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> entity)
    {
        entity.ToTable("Permission");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.PermissionKey).HasMaxLength(100);
        entity.Property(e => e.Description).HasMaxLength(255);
        entity.Property(e => e.Module).HasMaxLength(50);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => e.PermissionKey).IsUnique();
    }
}