using DoAn.HotelParking.Core.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Auth;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("User");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.LastName).HasMaxLength(100);
        entity.Property(e => e.FirstName).HasMaxLength(100);
        entity.Property(e => e.Email).HasMaxLength(200);
        entity.Property(e => e.Phone).HasMaxLength(15).IsUnicode(false);
        entity.Property(e => e.Password).HasMaxLength(255).IsUnicode(false);
        entity.Property(e => e.AvatarUrl).IsUnicode(false);
        entity.Property(e => e.Status).HasConversion<byte>();
        entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => e.Email).IsUnique();

        entity.HasOne(e => e.DeletedByUser)
            .WithMany()
            .HasForeignKey(e => e.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}