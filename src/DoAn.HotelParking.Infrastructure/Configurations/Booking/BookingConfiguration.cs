using DoAn.HotelParking.Core.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Booking;

public class BookingConfiguration : IEntityTypeConfiguration<DoAn.HotelParking.Core.Domain.Entities.Booking.Booking>
{
    public void Configure(EntityTypeBuilder<DoAn.HotelParking.Core.Domain.Entities.Booking.Booking> entity)
    {
        entity.ToTable("Booking", tb =>
        {
            tb.HasCheckConstraint("CK_Booking_TotalAmount", "[TotalAmount] >= 0");
            tb.HasCheckConstraint("CK_Booking_PaidAmount", "[PaidAmount] >= 0");
            tb.HasCheckConstraint("CK_Booking_PaidAmountNotOverTotal", "[PaidAmount] <= [TotalAmount]");
            tb.HasCheckConstraint("CK_Booking_GuestCount", "[GuestCount] >= 1");
            tb.HasCheckConstraint("CK_Booking_NightCount", "[NightCount] >= 1");
            tb.HasCheckConstraint("CK_Booking_CheckOutAfterCheckIn", "[CheckOutDate] > [CheckInDate]");
        });

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.RoomUnitPrice).HasColumnType("decimal(10,2)");
        entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)");
        entity.Property(e => e.PaidAmount).HasColumnType("decimal(10,2)");
        entity.Property(e => e.Note).HasMaxLength(255);
        entity.Property(e => e.CancelReason).HasMaxLength(255);
        entity.Property(e => e.Status).HasConversion<byte>();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => e.CustomerId);
        entity.HasIndex(e => e.RoomId);
        entity.HasIndex(e => e.TimeSlotId);
        entity.HasIndex(e => e.Status);
        entity.HasIndex(e => e.CheckInDate);
        entity.HasIndex(e => e.CheckOutDate);

        entity.HasOne(e => e.Room)
            .WithMany(e => e.Bookings)
            .HasForeignKey(e => e.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.Customer)
            .WithMany()
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.TimeSlot)
            .WithMany(e => e.Bookings)
            .HasForeignKey(e => e.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.CancelledByUser)
            .WithMany()
            .HasForeignKey(e => e.CancelledBy)
            .OnDelete(DeleteBehavior.NoAction);
    }
}