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
                Name = "Owner",
                Description = "Hotel owner",
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
            new Permission { Id = 3, PermissionKey = "hotel.read", Description = "Read hotels", Module = "Hotel", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 4, PermissionKey = "hotel.manage", Description = "Manage hotels", Module = "Hotel", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 5, PermissionKey = "room.read", Description = "Read rooms", Module = "Room", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 6, PermissionKey = "room.manage", Description = "Manage rooms", Module = "Room", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 7, PermissionKey = "booking.read", Description = "Read bookings", Module = "Booking", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 8, PermissionKey = "booking.manage", Description = "Manage bookings", Module = "Booking", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 9, PermissionKey = "payment.read", Description = "Read payments", Module = "Payment", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 10, PermissionKey = "payment.manage", Description = "Manage payments", Module = "Payment", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 11, PermissionKey = "review.read", Description = "Read reviews", Module = "Review", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 12, PermissionKey = "review.manage", Description = "Manage reviews", Module = "Review", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 13, PermissionKey = "notification.read", Description = "Read notifications", Module = "Notification", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 14, PermissionKey = "location.read", Description = "Read locations", Module = "Location", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 15, PermissionKey = "location.manage", Description = "Manage locations", Module = "Location", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 16, PermissionKey = "timeslot.read", Description = "Read time slots", Module = "TimeSlot", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 17, PermissionKey = "timeslot.manage", Description = "Manage time slots", Module = "TimeSlot", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 18, PermissionKey = "hotelimage.read", Description = "Read hotel images", Module = "HotelImage", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 19, PermissionKey = "hotelimage.manage", Description = "Manage hotel images", Module = "HotelImage", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 20, PermissionKey = "notification.manage", Description = "Manage notifications", Module = "Notification", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 21, PermissionKey = "permission.read", Description = "Read permissions", Module = "Permission", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Permission { Id = 22, PermissionKey = "permission.manage", Description = "Manage permissions", Module = "Permission", CreatedAt = seededAt, UpdatedAt = seededAt });

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
            new RolePermission { RoleId = 1, PermissionId = 10, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 11, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 12, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 13, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 14, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 15, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 16, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 17, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 18, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 19, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 20, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 21, CreatedAt = seededAt },
            new RolePermission { RoleId = 1, PermissionId = 22, CreatedAt = seededAt },

            new RolePermission { RoleId = 2, PermissionId = 3, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 4, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 5, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 6, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 7, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 8, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 9, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 11, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 13, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 14, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 16, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 17, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 18, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 19, CreatedAt = seededAt },
            new RolePermission { RoleId = 2, PermissionId = 20, CreatedAt = seededAt },

            new RolePermission { RoleId = 3, PermissionId = 3, CreatedAt = seededAt },
            new RolePermission { RoleId = 3, PermissionId = 5, CreatedAt = seededAt },
            new RolePermission { RoleId = 3, PermissionId = 7, CreatedAt = seededAt },
            new RolePermission { RoleId = 3, PermissionId = 8, CreatedAt = seededAt },
            new RolePermission { RoleId = 3, PermissionId = 11, CreatedAt = seededAt },
            new RolePermission { RoleId = 3, PermissionId = 12, CreatedAt = seededAt },
            new RolePermission { RoleId = 3, PermissionId = 13, CreatedAt = seededAt },
            new RolePermission { RoleId = 3, PermissionId = 14, CreatedAt = seededAt },
            new RolePermission { RoleId = 3, PermissionId = 16, CreatedAt = seededAt },
            new RolePermission { RoleId = 3, PermissionId = 18, CreatedAt = seededAt });

        modelBuilder.Entity<RoomType>().HasData(
            new RoomType { Id = 1, Name = "Standard", Description = "Standard room" },
            new RoomType { Id = 2, Name = "Deluxe", Description = "Deluxe room" });
    }
}