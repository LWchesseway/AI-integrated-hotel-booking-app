using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Hotel;

public class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
{
    public void Configure(EntityTypeBuilder<RoomType> entity)
    {
        entity.ToTable("RoomType");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.Name).HasMaxLength(100);
        entity.Property(e => e.Description).HasMaxLength(255);
    }
}