using DoAn.HotelParking.Core.Domain.Entities.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Location;

public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> entity)
    {
        entity.ToTable("Province");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
        entity.Property(e => e.Code).HasMaxLength(20).IsUnicode(false);
        entity.Property(e => e.IsActive).HasDefaultValue(true);

        entity.HasIndex(e => e.Name).IsUnique();
        entity.HasIndex(e => e.Code).IsUnique();
    }
}
