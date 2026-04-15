using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Hotel;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> entity)
    {
        entity.ToTable("Room", tb =>
        {
            tb.HasCheckConstraint("CK_Room_Price", "[Price] >= 0");
        });

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.RoomNumber).HasMaxLength(50);
        entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
        entity.Property(e => e.Status).HasConversion<byte>();
        entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => new { e.HotelId, e.RoomNumber }).IsUnique();
        entity.HasIndex(e => e.HotelId);
        entity.HasIndex(e => e.Status);

        entity.HasOne(e => e.Hotel)
            .WithMany(e => e.Rooms)
            .HasForeignKey(e => e.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.RoomType)
            .WithMany(e => e.Rooms)
            .HasForeignKey(e => e.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}