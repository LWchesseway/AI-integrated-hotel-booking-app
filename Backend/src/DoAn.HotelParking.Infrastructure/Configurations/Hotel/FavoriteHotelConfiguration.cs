using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Hotel;

public class FavoriteHotelConfiguration : IEntityTypeConfiguration<FavoriteHotel>
{
    public void Configure(EntityTypeBuilder<FavoriteHotel> entity)
    {
        entity.ToTable("FavoriteHotel");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => new { e.UserId, e.HotelId }).IsUnique();

        entity.HasOne(e => e.User)
            .WithMany(e => e.FavoriteHotels)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.Hotel)
            .WithMany(e => e.FavoriteHotels)
            .HasForeignKey(e => e.HotelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
