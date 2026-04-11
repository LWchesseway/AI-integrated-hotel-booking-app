using DoAn.HotelParking.Core.Domain.Entities.Auth;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Data.Seeding;

public static class ModelBuilderSeedDataExtensions
{
    public static void SeedReferenceData(this ModelBuilder modelBuilder)
    {
        var seededAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = 1,
                Name = "Admin",
                Description = "System administrator",
                IsActive = true,
                CreatedAt = seededAt,
                UpdatedAt = seededAt
            },
            new Role
            {
                Id = 2,
                Name = "Staff",
                Description = "Hotel staff",
                IsActive = true,
                CreatedAt = seededAt,
                UpdatedAt = seededAt
            },
            new Role
            {
                Id = 3,
                Name = "Customer",
                Description = "Hotel customer",
                IsActive = true,
                CreatedAt = seededAt,
                UpdatedAt = seededAt
            });

        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 1, PermissionKey = "user.read", Description = "Read users", Module = "User", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 2, PermissionKey = "user.manage", Description = "Manage users", Module = "User", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 3, PermissionKey = "room.read", Description = "Read rooms", Module = "Room", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 4, PermissionKey = "room.manage", Description = "Manage rooms", Module = "Room", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 5, PermissionKey = "booking.read", Description = "Read bookings", Module = "Booking", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 6, PermissionKey = "booking.manage", Description = "Manage bookings", Module = "Booking", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 7, PermissionKey = "parking.read", Description = "Read parking sessions", Module = "Parking", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 8, PermissionKey = "parking.manage", Description = "Manage parking sessions", Module = "Parking", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 9, PermissionKey = "notification.read", Description = "Read notifications", Module = "Notification", CreatedAt = seededAt, UpdatedAt = seededAt });

        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 1, PermissionId = 1, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 2, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 3, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 4, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 5, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 6, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 7, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 8, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 9, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 3, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 5, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 6, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 7, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 8, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 9, CreatedAt = seededAt },
            new RolePermission { RoleId = 3, PermissionId = 3, CreatedAt = seededAt },
            new RolePermission { RoleId = 3, PermissionId = 5, CreatedAt = seededAt },
            new RolePermission { RoleId = 3, PermissionId = 9, CreatedAt = seededAt });

        modelBuilder.Entity<RoomType>().HasData(
            new RoomType { Id = 1, Name = "Standard", Description = "Standard room" },
            new RoomType { Id = 2, Name = "Deluxe", Description = "Deluxe room" },
            new RoomType { Id = 3, Name = "Suite", Description = "Suite room" });
    }
}