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
            tb.HasCheckConstraint("CK_Booking_DepositAmount", "[DepositAmount] >= 0");
            tb.HasCheckConstraint("CK_Booking_DepositLessThanTotal", "[DepositAmount] <= [TotalAmount]");
        });

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)");
        entity.Property(e => e.DepositAmount).HasColumnType("decimal(10,2)");
        entity.Property(e => e.PaymentProofUrl).IsUnicode(false);
        entity.Property(e => e.Note).HasMaxLength(255);
        entity.Property(e => e.Status).HasConversion<byte>();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => e.CustomerId);
        entity.HasIndex(e => e.RoomId);
        entity.HasIndex(e => e.Status);
        entity.HasIndex(e => e.CreatedAt);

        entity.HasOne(e => e.Room)
            .WithMany(e => e.Bookings)
            .HasForeignKey(e => e.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.Customer)
            .WithMany()
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.ApprovedByUser)
            .WithMany()
            .HasForeignKey(e => e.ApprovedBy)
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(e => e.CancelledByUser)
            .WithMany()
            .HasForeignKey(e => e.CancelledBy)
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(e => e.Payment)
            .WithOne(e => e.Booking)
            .HasForeignKey<Payment>(e => e.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}