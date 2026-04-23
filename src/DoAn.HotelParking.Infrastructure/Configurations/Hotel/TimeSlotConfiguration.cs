using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Hotel;

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> entity)
    {
        entity.ToTable("TimeSlot", tb =>
        {
            tb.HasCheckConstraint("CK_TimeSlot_DateRange", "[EndDate] > [StartDate]");
            tb.HasCheckConstraint("CK_TimeSlot_Price", "[Price] >= 0");
        });

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.StartDate).HasColumnType("date");
        entity.Property(e => e.EndDate).HasColumnType("date");
        entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => e.RoomId);
        entity.HasIndex(e => new { e.RoomId, e.StartDate, e.EndDate }).IsUnique();

        entity.HasOne(e => e.Room)
            .WithMany(e => e.TimeSlots)
            .HasForeignKey(e => e.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
