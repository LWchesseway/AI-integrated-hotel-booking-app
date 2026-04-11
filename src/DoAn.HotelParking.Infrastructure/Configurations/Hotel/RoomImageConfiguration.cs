using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Hotel;

public class RoomImageConfiguration : IEntityTypeConfiguration<RoomImage>
{
    public void Configure(EntityTypeBuilder<RoomImage> entity)
    {
        entity.ToTable("RoomImage");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.ImageUrl).IsUnicode(false);

        entity.HasOne(e => e.Room)
            .WithMany(e => e.RoomImages)
            .HasForeignKey(e => e.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}