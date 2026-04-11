using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Hotel;

public class HotelConfiguration : IEntityTypeConfiguration<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel>
{
    public void Configure(EntityTypeBuilder<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel> entity)
    {
        entity.ToTable("Hotel");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.Name).HasMaxLength(255);
        entity.Property(e => e.Street).HasMaxLength(100);
        entity.Property(e => e.Ward).HasMaxLength(100);
        entity.Property(e => e.Province).HasMaxLength(100);
        entity.Property(e => e.Phone).HasMaxLength(15).IsUnicode(false);
        entity.Property(e => e.Description).HasMaxLength(500);
        entity.Property(e => e.Status).HasConversion<byte>();
        entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => e.IsDeleted);
    }
}