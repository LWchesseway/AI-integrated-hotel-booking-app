using DoAn.HotelParking.Core.Domain.Entities.Parking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Parking;

public class ParkingSessionConfiguration : IEntityTypeConfiguration<ParkingSession>
{
    public void Configure(EntityTypeBuilder<ParkingSession> entity)
    {
        entity.ToTable("ParkingSession");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.CheckInPlate).HasMaxLength(20).IsUnicode(false);
        entity.Property(e => e.CheckOutPlate).HasMaxLength(20).IsUnicode(false);
        entity.Property(e => e.Status).HasConversion<byte>();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => e.UserId);
        entity.HasIndex(e => e.Status);
        entity.HasIndex(e => e.VerifiedBy);

        entity.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.QrUser)
            .WithMany()
            .HasForeignKey(e => e.QrUserId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.VerifiedByUser)
            .WithMany()
            .HasForeignKey(e => e.VerifiedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}