using DoAn.HotelParking.Core.Domain.Entities.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Location;

public class WardConfiguration : IEntityTypeConfiguration<Ward>
{
    public void Configure(EntityTypeBuilder<Ward> entity)
    {
        entity.ToTable("Ward");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
        entity.Property(e => e.Code).HasMaxLength(20).IsUnicode(false);
        entity.Property(e => e.IsActive).HasDefaultValue(true);

        entity.HasIndex(e => new { e.ProvinceId, e.Name }).IsUnique();
        entity.HasIndex(e => e.Code).IsUnique();

        entity.HasOne(e => e.Province)
            .WithMany(e => e.Wards)
            .HasForeignKey(e => e.ProvinceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
