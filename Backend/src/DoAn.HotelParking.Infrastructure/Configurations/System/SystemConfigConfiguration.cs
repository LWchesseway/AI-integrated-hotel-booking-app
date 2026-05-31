using DoAn.HotelParking.Core.Domain.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.System;

public class SystemConfigConfiguration : IEntityTypeConfiguration<SystemConfig>
{
    public void Configure(EntityTypeBuilder<SystemConfig> entity)
    {
        entity.ToTable("SystemConfig", tb =>
        {
            tb.HasCheckConstraint(
                "CK_SystemConfig_DataType",
                "[DataType] IN ('string', 'int', 'decimal', 'boolean', 'datetime')");
        });

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.ConfigKey).HasMaxLength(100).IsRequired();
        entity.Property(e => e.ConfigValue).HasMaxLength(255);
        entity.Property(e => e.DataType).HasMaxLength(20).HasDefaultValue("string").IsUnicode(false);
        entity.Property(e => e.Description).HasMaxLength(255);
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => e.ConfigKey).IsUnique();
    }
}
