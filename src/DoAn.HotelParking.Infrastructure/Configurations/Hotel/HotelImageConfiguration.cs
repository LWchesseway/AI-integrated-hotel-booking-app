using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Hotel;

public class HotelImageConfiguration : IEntityTypeConfiguration<HotelImage>
{
    public void Configure(EntityTypeBuilder<HotelImage> entity)
    {
        entity.ToTable("HotelImage");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.ImageUrl).HasMaxLength(1000).IsUnicode(false);
        entity.Property(e => e.ObjectKey).HasMaxLength(500).IsUnicode(false);
        entity.Property(e => e.IsPrimary).HasDefaultValue(false);
        entity.Property(e => e.SortOrder).HasDefaultValue(0);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => e.HotelId);

        entity.HasOne(e => e.Hotel)
            .WithMany(e => e.HotelImages)
            .HasForeignKey(e => e.HotelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
