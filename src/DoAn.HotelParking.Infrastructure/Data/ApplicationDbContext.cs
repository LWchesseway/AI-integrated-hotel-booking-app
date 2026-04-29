using DoAn.HotelParking.Core.Domain.Entities.Auth;
using DoAn.HotelParking.Core.Domain.Entities.Booking;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using DoAn.HotelParking.Core.Domain.Entities.Location;
using DoAn.HotelParking.Core.Domain.Entities.Notification;
using DoAn.HotelParking.Core.Domain.Entities.OwnerSetting;
using DoAn.HotelParking.Core.Domain.Entities.Review;
using DoAn.HotelParking.Core.Domain.Entities.System;
using DoAn.HotelParking.Infrastructure.Data.Seeding;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<HotelImage> HotelImages => Set<HotelImage>();
    public DbSet<FavoriteHotel> FavoriteHotels => Set<FavoriteHotel>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<Ward> Wards => Set<Ward>();
    public DbSet<OwnerSetting> OwnerSettings => Set<OwnerSetting>();
    public DbSet<SystemConfig> SystemConfigs => Set<SystemConfig>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Review> Reviews => Set<Review>();
    //public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.SeedReferenceData();
    }

    public override int SaveChanges()
    {
        UpdateAuditProperties();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditProperties();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditProperties()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is not EntityState.Added and not EntityState.Modified)
            {
                continue;
            }

            var createdAt = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedAt");
            var updatedAt = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");

            if (entry.State == EntityState.Added && createdAt is not null)
            {
                if (createdAt.CurrentValue is null || (DateTime)createdAt.CurrentValue == default)
                {
                    createdAt.CurrentValue = now;
                }
            }

            if (updatedAt is not null)
            {
                updatedAt.CurrentValue = now;
            }
        }
    }
}