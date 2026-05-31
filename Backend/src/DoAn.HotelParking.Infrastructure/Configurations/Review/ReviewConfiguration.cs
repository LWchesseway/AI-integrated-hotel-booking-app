using DoAn.HotelParking.Core.Domain.Entities.Review;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Review;

public class ReviewConfiguration : IEntityTypeConfiguration<DoAn.HotelParking.Core.Domain.Entities.Review.Review>
{
    public void Configure(EntityTypeBuilder<DoAn.HotelParking.Core.Domain.Entities.Review.Review> entity)
    {
        entity.ToTable("Review", tb =>
        {
            tb.HasCheckConstraint("CK_Review_Rating", "[Rating] >= 1 AND [Rating] <= 5");
        });

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.Comment).HasMaxLength(500);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => e.BookingId);
        entity.HasIndex(e => e.CustomerId);
        entity.HasIndex(e => e.RoomId);

        entity.HasOne(e => e.Booking)
            .WithMany(e => e.Reviews)
            .HasForeignKey(e => e.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.Customer)
            .WithMany()
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.Room)
            .WithMany(e => e.Reviews)
            .HasForeignKey(e => e.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}