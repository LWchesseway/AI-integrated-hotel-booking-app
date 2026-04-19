using DoAn.HotelParking.Core.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Booking;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> entity)
    {
        entity.ToTable("Payment");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.Amount).HasColumnType("decimal(10,2)");
        entity.Property(e => e.Method).HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.Status).HasConversion<byte>();
        entity.Property(e => e.TransactionCode).HasMaxLength(100).IsUnicode(false);
        entity.Property(e => e.Note).HasMaxLength(255);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => e.BookingId);
        entity.HasIndex(e => e.TransactionCode)
            .IsUnique()
            .HasFilter("[TransactionCode] IS NOT NULL");

        entity.HasOne(e => e.Booking)
            .WithMany(e => e.Payments)
            .HasForeignKey(e => e.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}