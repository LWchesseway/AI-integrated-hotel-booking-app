using DoAn.HotelParking.Core.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Auth;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> entity)
    {
        entity.ToTable("Role");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.Name).HasMaxLength(50);
        entity.Property(e => e.Description).HasMaxLength(255);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
    }
}