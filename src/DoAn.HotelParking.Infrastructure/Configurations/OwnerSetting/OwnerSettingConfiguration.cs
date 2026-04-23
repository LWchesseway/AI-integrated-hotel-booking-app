using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OwnerSettingEntity = DoAn.HotelParking.Core.Domain.Entities.OwnerSetting.OwnerSetting;

namespace DoAn.HotelParking.Infrastructure.Configurations.OwnerSetting;

public class OwnerSettingConfiguration : IEntityTypeConfiguration<OwnerSettingEntity>
{
    public void Configure(EntityTypeBuilder<OwnerSettingEntity> entity)
    {
        entity.ToTable("OwnerSetting", tb =>
        {
            tb.HasCheckConstraint("CK_OwnerSetting_DepositRate", "[DepositRate] IS NULL OR ([DepositRate] >= 0 AND [DepositRate] <= 1)");
            tb.HasCheckConstraint("CK_OwnerSetting_MinBookingNotice", "[MinBookingNotice] IS NULL OR [MinBookingNotice] >= 0");
        });

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.DepositRate).HasColumnType("decimal(5,4)");
        entity.Property(e => e.BankName).HasMaxLength(100);
        entity.Property(e => e.BankAccountNumber).HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.BankAccountName).HasMaxLength(200);
        entity.Property(e => e.BankQrCodeUrl).HasMaxLength(500).IsUnicode(false);
        entity.Property(e => e.AllowReview).HasDefaultValue(true);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => e.OwnerId).IsUnique();

        entity.HasOne(e => e.Owner)
            .WithMany(e => e.OwnerSettings)
            .HasForeignKey(e => e.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
