using DoAn.HotelParking.Core.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Auth;

public class QrCodeConfiguration : IEntityTypeConfiguration<QrCode>
{
    public void Configure(EntityTypeBuilder<QrCode> entity)
    {
        entity.ToTable("QrCode");

        entity.HasKey(e => e.Code);

        entity.Property(e => e.Code).HasMaxLength(255).IsUnicode(false);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasOne(e => e.User)
            .WithMany(e => e.QrCodes)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}