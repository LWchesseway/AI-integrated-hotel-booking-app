using DoAn.HotelParking.Core.Domain.Entities.Parking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Parking;

public class LicensePlateLogConfiguration : IEntityTypeConfiguration<LicensePlateLog>
{
    public void Configure(EntityTypeBuilder<LicensePlateLog> entity)
    {
        entity.ToTable("LicensePlateLog", tb =>
        {
            tb.HasCheckConstraint("CK_LicensePlateLog_Confidence", "[Confidence] >= 0 AND [Confidence] <= 1");
        });

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.ImageUrl).IsUnicode(false);
        entity.Property(e => e.DetectedPlate).HasMaxLength(20).IsUnicode(false);
        entity.Property(e => e.Confidence).HasColumnType("decimal(3,2)");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => new { e.ParkingSessionId, e.CreatedAt });

        entity.HasOne(e => e.ParkingSession)
            .WithMany(e => e.LicensePlateLogs)
            .HasForeignKey(e => e.ParkingSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}