using DoAn.HotelParking.Core.Domain.Entities.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Notification;

public class NotificationConfiguration : IEntityTypeConfiguration<DoAn.HotelParking.Core.Domain.Entities.Notification.Notification>
{
    public void Configure(EntityTypeBuilder<DoAn.HotelParking.Core.Domain.Entities.Notification.Notification> entity)
    {
        entity.ToTable("Notification");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.Title).HasMaxLength(255);
        entity.Property(e => e.Type).HasConversion<byte>();
        entity.Property(e => e.RelatedTable).HasMaxLength(100);
        entity.Property(e => e.IsRead).HasDefaultValue(false);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => e.UserId);
        entity.HasIndex(e => e.IsRead);

        entity.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(e => e.Sender)
            .WithMany()
            .HasForeignKey(e => e.SenderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}