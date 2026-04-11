using DoAn.HotelParking.Core.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAn.HotelParking.Infrastructure.Configurations.Auth;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entity)
    {
        entity.ToTable("RefreshToken");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.Token).HasMaxLength(256).IsUnicode(false);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        entity.HasIndex(e => e.Token).IsUnique();
        entity.HasIndex(e => e.UserId);
        entity.HasIndex(e => e.IsRevoked);

        entity.HasOne(e => e.User)
            .WithMany(e => e.RefreshTokens)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}