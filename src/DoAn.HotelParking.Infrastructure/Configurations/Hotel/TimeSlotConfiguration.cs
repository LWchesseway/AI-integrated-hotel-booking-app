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
            tb.HasCheckConstraint("CK_TimeSlot_MinStayNights", "[MinStayNights] >= 1");
            tb.HasCheckConstraint("CK_TimeSlot_CancellationHours", "[CancellationHoursBeforeCheckIn] >= 0");
            tb.HasCheckConstraint("CK_TimeSlot_MaxStayNights", "[MaxStayNights] IS NULL OR [MaxStayNights] >= [MinStayNights]");
        });

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.Name).HasMaxLength(100);
        entity.Property(e => e.CheckInFrom).HasColumnType("time");
        entity.Property(e => e.CheckOutUntil).HasColumnType("time");
        entity.Property(e => e.IsDefault).HasDefaultValue(false);
        entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => e.HotelId);
        entity.HasIndex(e => new { e.HotelId, e.IsDefault });

        entity.HasOne(e => e.Hotel)
            .WithMany(e => e.TimeSlots)
            .HasForeignKey(e => e.HotelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
