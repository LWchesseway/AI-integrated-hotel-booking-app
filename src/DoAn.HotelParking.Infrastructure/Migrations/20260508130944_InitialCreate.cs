using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAn.HotelParking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermissionKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Module = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Province",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Province", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConfigValue = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DataType = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "string"),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfig", x => x.Id);
                    table.CheckConstraint("CK_SystemConfig_DataType", "[DataType] IN ('string', 'int', 'decimal', 'boolean', 'datetime')");
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    AvatarUrl = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "SYSUTCDATETIME()"),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_User_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ward",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ward", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ward_Province_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Province",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FcmToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "varchar(512)", unicode: false, maxLength: 512, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FcmToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FcmToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SenderId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    RelatedTable = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RelatedId = table.Column<int>(type: "int", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_User_SenderId",
                        column: x => x.SenderId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notification_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OwnerSetting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    DepositRate = table.Column<decimal>(type: "decimal(5,4)", nullable: true),
                    MinBookingNotice = table.Column<int>(type: "int", nullable: true),
                    AllowReview = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankAccountNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    BankAccountName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BankQrCodeUrl = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerSetting", x => x.Id);
                    table.CheckConstraint("CK_OwnerSetting_DepositRate", "[DepositRate] IS NULL OR ([DepositRate] >= 0 AND [DepositRate] <= 1)");
                    table.CheckConstraint("CK_OwnerSetting_MinBookingNotice", "[MinBookingNotice] IS NULL OR [MinBookingNotice] >= 0");
                    table.ForeignKey(
                        name: "FK_OwnerSetting_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hotel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    WardId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hotel_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Hotel_Ward_WardId",
                        column: x => x.WardId,
                        principalTable: "Ward",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteHotel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteHotel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteHotel_Hotel_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteHotel_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true),
                    ObjectKey = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelImage_Hotel_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    RoomTypeId = table.Column<int>(type: "int", nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                    table.CheckConstraint("CK_Room_Price", "[Price] >= 0");
                    table.ForeignKey(
                        name: "FK_Room_Hotel_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Room_RoomType_RoomTypeId",
                        column: x => x.RoomTypeId,
                        principalTable: "RoomType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NightCount = table.Column<int>(type: "int", nullable: false),
                    GuestCount = table.Column<int>(type: "int", nullable: false),
                    RoomUnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CancelledBy = table.Column<int>(type: "int", nullable: true),
                    CancelReason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.Id);
                    table.CheckConstraint("CK_Booking_CheckOutAfterCheckIn", "[CheckOutDate] > [CheckInDate]");
                    table.CheckConstraint("CK_Booking_GuestCount", "[GuestCount] >= 1");
                    table.CheckConstraint("CK_Booking_NightCount", "[NightCount] >= 1");
                    table.CheckConstraint("CK_Booking_PaidAmount", "[PaidAmount] >= 0");
                    table.CheckConstraint("CK_Booking_PaidAmountNotOverTotal", "[PaidAmount] <= [TotalAmount]");
                    table.CheckConstraint("CK_Booking_TotalAmount", "[TotalAmount] >= 0");
                    table.ForeignKey(
                        name: "FK_Booking_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Booking_User_CancelledBy",
                        column: x => x.CancelledBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Booking_User_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlot", x => x.Id);
                    table.CheckConstraint("CK_TimeSlot_DateRange", "[EndDate] > [StartDate]");
                    table.CheckConstraint("CK_TimeSlot_Price", "[Price] >= 0");
                    table.ForeignKey(
                        name: "FK_TimeSlot_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Method = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    TransactionCode = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Booking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<byte>(type: "tinyint", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                    table.CheckConstraint("CK_Review_Rating", "[Rating] >= 1 AND [Rating] <= 5");
                    table.ForeignKey(
                        name: "FK_Review_Booking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Review_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Review_User_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "CreatedAt", "Description", "Module", "PermissionKey", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read users", "User", "user.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage users", "User", "user.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read hotels", "Hotel", "hotel.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage hotels", "Hotel", "hotel.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read rooms", "Room", "room.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage rooms", "Room", "room.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read bookings", "Booking", "booking.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage bookings", "Booking", "booking.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read payments", "Payment", "payment.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage payments", "Payment", "payment.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read reviews", "Review", "review.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage reviews", "Review", "review.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read notifications", "Notification", "notification.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read locations", "Location", "location.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage locations", "Location", "location.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read time slots", "TimeSlot", "timeslot.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage time slots", "TimeSlot", "timeslot.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read hotel images", "HotelImage", "hotelimage.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage hotel images", "HotelImage", "hotelimage.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage notifications", "Notification", "notification.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read permissions", "Permission", "permission.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage permissions", "Permission", "permission.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read owner settings", "OwnerSetting", "ownersetting.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage owner settings", "OwnerSetting", "ownersetting.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage system configuration", "SystemConfig", "system.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read owner statistics", "Statistics", "statistics.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read recommendations", "Recommendation", "recommendation.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 28, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage favorite hotels", "Favorite", "favorite.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 29, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Force complete booking", "Booking", "booking.force_complete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System administrator", true, "Admin", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hotel owner", true, "Owner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hotel customer", true, "Customer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "RoomType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Standard room", "Standard" },
                    { 2, "Deluxe room", "Deluxe" }
                });

            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[] { "Id", "ConfigKey", "ConfigValue", "DataType", "Description", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "DEFAULT_DEPOSIT_RATE", "0.30", "decimal", "Default booking deposit rate", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "MIN_BOOKING_NOTICE_HOURS", "2", "int", "Minimum hours before check-in to allow booking", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "ENABLE_REVIEW_SYSTEM", "true", "boolean", "Enable reviews and ratings", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "AvatarUrl", "CreatedAt", "DeletedAt", "DeletedBy", "Email", "FirstName", "LastName", "Password", "Phone", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "https://example.com/avatars/admin-01.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "admin01@demo.local", "Admin", "User01", "Password123!", "0900000001", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "https://example.com/avatars/admin-02.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "admin02@demo.local", "Admin", "User02", "Password123!", "0900000002", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "https://example.com/avatars/owner-01.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "owner01@demo.local", "Owner", "User01", "Password123!", "0910000003", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, "https://example.com/avatars/owner-02.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "owner02@demo.local", "Owner", "User02", "Password123!", "0910000004", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, "https://example.com/avatars/owner-03.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "owner03@demo.local", "Owner", "User03", "Password123!", "0910000005", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, "https://example.com/avatars/owner-04.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "owner04@demo.local", "Owner", "User04", "Password123!", "0910000006", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, "https://example.com/avatars/owner-05.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "owner05@demo.local", "Owner", "User05", "Password123!", "0910000007", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, "https://example.com/avatars/customer-01.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer01@demo.local", "Customer", "User01", "Password123!", "0920000008", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, "https://example.com/avatars/customer-02.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer02@demo.local", "Customer", "User02", "Password123!", "0920000009", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, "https://example.com/avatars/customer-03.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer03@demo.local", "Customer", "User03", "Password123!", "0920000010", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, "https://example.com/avatars/customer-04.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer04@demo.local", "Customer", "User04", "Password123!", "0920000011", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, "https://example.com/avatars/customer-05.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer05@demo.local", "Customer", "User05", "Password123!", "0920000012", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, "https://example.com/avatars/customer-06.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer06@demo.local", "Customer", "User06", "Password123!", "0920000013", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, "https://example.com/avatars/customer-07.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer07@demo.local", "Customer", "User07", "Password123!", "0920000014", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, "https://example.com/avatars/customer-08.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer08@demo.local", "Customer", "User08", "Password123!", "0920000015", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, "https://example.com/avatars/customer-09.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer09@demo.local", "Customer", "User09", "Password123!", "0920000016", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, "https://example.com/avatars/customer-10.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer10@demo.local", "Customer", "User10", "Password123!", "0920000017", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, "https://example.com/avatars/customer-11.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer11@demo.local", "Customer", "User11", "Password123!", "0920000018", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, "https://example.com/avatars/customer-12.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer12@demo.local", "Customer", "User12", "Password123!", "0920000019", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, "https://example.com/avatars/customer-13.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer13@demo.local", "Customer", "User13", "Password123!", "0920000020", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, "https://example.com/avatars/customer-14.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer14@demo.local", "Customer", "User14", "Password123!", "0920000021", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, "https://example.com/avatars/customer-15.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer15@demo.local", "Customer", "User15", "Password123!", "0920000022", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, "https://example.com/avatars/customer-16.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer16@demo.local", "Customer", "User16", "Password123!", "0920000023", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, "https://example.com/avatars/customer-17.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer17@demo.local", "Customer", "User17", "Password123!", "0920000024", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, "https://example.com/avatars/customer-18.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer18@demo.local", "Customer", "User18", "Password123!", "0920000025", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, "https://example.com/avatars/customer-19.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer19@demo.local", "Customer", "User19", "Password123!", "0920000026", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, "https://example.com/avatars/customer-20.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "customer20@demo.local", "Customer", "User20", "Password123!", "0920000027", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Hotel",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "OwnerId", "Phone", "Status", "Street", "UpdatedAt", "WardId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeded hotel for testing", "Hotel 1", 3, "0910000001", (byte)1, "11 Main Street", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeded hotel for testing", "Hotel 2", 3, "0910000002", (byte)1, "12 Main Street", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeded hotel for testing", "Hotel 3", 4, "0910000003", (byte)1, "13 Main Street", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeded hotel for testing", "Hotel 4", 4, "0910000004", (byte)1, "14 Main Street", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeded hotel for testing", "Hotel 5", 5, "0910000005", (byte)1, "15 Main Street", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5 },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeded hotel for testing", "Hotel 6", 5, "0910000006", (byte)1, "16 Main Street", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6 },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeded hotel for testing", "Hotel 7", 6, "0910000007", (byte)1, "17 Main Street", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7 },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeded hotel for testing", "Hotel 8", 6, "0910000008", (byte)1, "18 Main Street", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8 },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeded hotel for testing", "Hotel 9", 7, "0910000009", (byte)1, "19 Main Street", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9 },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeded hotel for testing", "Hotel 10", 7, "0910000010", (byte)1, "20 Main Street", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10 }
                });

            migrationBuilder.InsertData(
                table: "OwnerSetting",
                columns: new[] { "Id", "AllowReview", "BankAccountName", "BankAccountNumber", "BankName", "BankQrCodeUrl", "CreatedAt", "DepositRate", "MinBookingNotice", "OwnerId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, true, "Owner 1", "0123456700", "Vietcombank", "https://example.com/bank/owner-1.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.20m, 1, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, true, "Owner 2", "0123456701", "Vietcombank", "https://example.com/bank/owner-2.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.22m, 2, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, true, "Owner 3", "0123456702", "Vietcombank", "https://example.com/bank/owner-3.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.24m, 3, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, true, "Owner 4", "0123456703", "Vietcombank", "https://example.com/bank/owner-4.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.26m, 1, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, true, "Owner 5", "0123456704", "Vietcombank", "https://example.com/bank/owner-5.png", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.28m, 2, 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "PermissionId", "RoleId", "CreatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 28, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 29, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 1, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 28, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 1, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 28, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 29, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "FavoriteHotel",
                columns: new[] { "Id", "CreatedAt", "HotelId", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 8 },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 8 },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 9 },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 9 },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 10 },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 10 },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 11 },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, 11 },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 12 },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 12 },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 13 },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, 13 },
                    { 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, 14 },
                    { 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, 14 },
                    { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 15 },
                    { 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 15 },
                    { 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, 16 },
                    { 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 16 },
                    { 19, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, 17 },
                    { 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 17 },
                    { 21, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 18 },
                    { 22, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 18 },
                    { 23, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 19 },
                    { 24, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 19 },
                    { 25, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 20 },
                    { 26, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 20 },
                    { 27, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 21 },
                    { 28, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, 21 },
                    { 29, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 22 },
                    { 30, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 22 },
                    { 31, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 23 },
                    { 32, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, 23 },
                    { 33, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, 24 },
                    { 34, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, 24 },
                    { 35, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 25 },
                    { 36, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 25 },
                    { 37, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, 26 },
                    { 38, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 26 },
                    { 39, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, 27 },
                    { 40, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 27 }
                });

            migrationBuilder.InsertData(
                table: "Room",
                columns: new[] { "Id", "Capacity", "CreatedAt", "HotelId", "Price", "RoomNumber", "RoomTypeId", "Status" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 95m, "101", 2, (byte)1 },
                    { 2, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 80m, "102", 1, (byte)1 },
                    { 3, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 105m, "103", 2, (byte)1 },
                    { 4, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 90m, "104", 1, (byte)1 },
                    { 5, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 115m, "105", 2, (byte)1 },
                    { 6, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 75m, "101", 1, (byte)1 },
                    { 7, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 100m, "102", 2, (byte)1 },
                    { 8, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 85m, "103", 1, (byte)1 },
                    { 9, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 110m, "104", 2, (byte)1 },
                    { 10, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 95m, "105", 1, (byte)1 },
                    { 11, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 95m, "101", 2, (byte)1 },
                    { 12, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 80m, "102", 1, (byte)1 },
                    { 13, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 105m, "103", 2, (byte)1 },
                    { 14, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 90m, "104", 1, (byte)1 },
                    { 15, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 115m, "105", 2, (byte)1 },
                    { 16, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 75m, "101", 1, (byte)1 },
                    { 17, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 100m, "102", 2, (byte)1 },
                    { 18, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 85m, "103", 1, (byte)1 },
                    { 19, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 110m, "104", 2, (byte)1 },
                    { 20, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 95m, "105", 1, (byte)1 },
                    { 21, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 95m, "101", 2, (byte)1 },
                    { 22, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 80m, "102", 1, (byte)1 },
                    { 23, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 105m, "103", 2, (byte)1 },
                    { 24, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 90m, "104", 1, (byte)1 },
                    { 25, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 115m, "105", 2, (byte)1 },
                    { 26, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 75m, "101", 1, (byte)1 },
                    { 27, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 100m, "102", 2, (byte)1 },
                    { 28, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 85m, "103", 1, (byte)1 },
                    { 29, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 110m, "104", 2, (byte)1 },
                    { 30, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 95m, "105", 1, (byte)1 },
                    { 31, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, 95m, "101", 2, (byte)1 },
                    { 32, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, 80m, "102", 1, (byte)1 },
                    { 33, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, 105m, "103", 2, (byte)1 },
                    { 34, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, 90m, "104", 1, (byte)1 },
                    { 35, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, 115m, "105", 2, (byte)1 },
                    { 36, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 75m, "101", 1, (byte)1 },
                    { 37, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 100m, "102", 2, (byte)1 },
                    { 38, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 85m, "103", 1, (byte)1 },
                    { 39, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 110m, "104", 2, (byte)1 },
                    { 40, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 95m, "105", 1, (byte)1 },
                    { 41, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, 95m, "101", 2, (byte)1 },
                    { 42, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, 80m, "102", 1, (byte)1 },
                    { 43, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, 105m, "103", 2, (byte)1 },
                    { 44, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, 90m, "104", 1, (byte)1 },
                    { 45, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, 115m, "105", 2, (byte)1 },
                    { 46, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, 75m, "101", 1, (byte)1 },
                    { 47, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, 100m, "102", 2, (byte)1 },
                    { 48, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, 85m, "103", 1, (byte)1 },
                    { 49, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, 110m, "104", 2, (byte)1 },
                    { 50, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, 95m, "105", 1, (byte)1 }
                });

            migrationBuilder.InsertData(
                table: "Booking",
                columns: new[] { "Id", "CancelReason", "CancelledAt", "CancelledBy", "CheckInDate", "CheckOutDate", "CreatedAt", "CustomerId", "GuestCount", "NightCount", "Note", "PaidAmount", "RoomId", "RoomUnitPrice", "Status", "TotalAmount", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, null, null, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 1, 1, "Seed booking", 0m, 1, 95m, (byte)0, 95m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, null, null, null, new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, 2, 2, "Seed booking", 80.0m, 2, 80m, (byte)1, 160m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "Seeded cancellation", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, 3, 3, "Seed booking", 0m, 3, 105m, (byte)2, 315m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, null, null, null, new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, 1, 1, "Seed booking", 90m, 4, 90m, (byte)3, 90m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, null, null, null, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 12, 2, 2, "Seed booking", 0m, 5, 115m, (byte)0, 230m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, null, null, null, new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 13, 2, 3, "Seed booking", 112.5m, 6, 75m, (byte)1, 225m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, "Seeded cancellation", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 14, 1, 1, "Seed booking", 0m, 7, 100m, (byte)2, 100m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, null, null, null, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 15, 2, 2, "Seed booking", 170m, 8, 85m, (byte)3, 170m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, null, null, null, new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 16, 2, 3, "Seed booking", 0m, 9, 110m, (byte)0, 330m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, null, null, null, new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, 1, 1, "Seed booking", 47.5m, 10, 95m, (byte)1, 95m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, "Seeded cancellation", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 18, 2, 2, "Seed booking", 0m, 11, 95m, (byte)2, 190m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, null, null, null, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 19, 3, 3, "Seed booking", 240m, 12, 80m, (byte)3, 240m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, null, null, null, new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 20, 1, 1, "Seed booking", 0m, 13, 105m, (byte)0, 105m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, null, null, null, new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 21, 2, 2, "Seed booking", 90.0m, 14, 90m, (byte)1, 180m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, "Seeded cancellation", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 22, 3, 3, "Seed booking", 0m, 15, 115m, (byte)2, 345m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, null, null, null, new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 23, 1, 1, "Seed booking", 75m, 16, 75m, (byte)3, 75m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, null, null, null, new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 24, 2, 2, "Seed booking", 0m, 17, 100m, (byte)0, 200m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, null, null, null, new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 25, 3, 3, "Seed booking", 127.5m, 18, 85m, (byte)1, 255m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, "Seeded cancellation", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 26, 1, 1, "Seed booking", 0m, 19, 110m, (byte)2, 110m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, null, null, null, new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 27, 2, 2, "Seed booking", 190m, 20, 95m, (byte)3, 190m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, null, null, null, new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 24, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 2, 3, "Seed booking", 0m, 21, 95m, (byte)0, 285m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, null, null, null, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, 1, 1, "Seed booking", 40.0m, 22, 80m, (byte)1, 80m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, "Seeded cancellation", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, 2, 2, "Seed booking", 0m, 23, 105m, (byte)2, 210m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, null, null, null, new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, 2, 3, "Seed booking", 270m, 24, 90m, (byte)3, 270m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, null, null, null, new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 12, 1, 1, "Seed booking", 0m, 25, 115m, (byte)0, 115m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, null, null, null, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 13, 2, 2, "Seed booking", 75.0m, 26, 75m, (byte)1, 150m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, "Seeded cancellation", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 14, 3, 3, "Seed booking", 0m, 27, 100m, (byte)2, 300m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 28, null, null, null, new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 15, 1, 1, "Seed booking", 85m, 28, 85m, (byte)3, 85m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 29, null, null, null, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 16, 2, 2, "Seed booking", 0m, 29, 110m, (byte)0, 220m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 30, null, null, null, new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, 3, 3, "Seed booking", 142.5m, 30, 95m, (byte)1, 285m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "TimeSlot",
                columns: new[] { "Id", "CreatedAt", "EndDate", "IsActive", "Price", "RoomId", "StartDate", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), true, 95m, 1, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), true, 105m, 1, new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, 80m, 2, new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), true, 90m, 2, new DateTime(2026, 5, 9, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, 105m, 3, new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), true, 115m, 3, new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), true, 90m, 4, new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 13, 0, 0, 0, 0, DateTimeKind.Utc), true, 100m, 4, new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 7, 0, 0, 0, 0, DateTimeKind.Utc), true, 115m, 5, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 125m, 5, new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), true, 75m, 6, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), true, 85m, 6, new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, 100m, 7, new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), true, 110m, 7, new DateTime(2026, 5, 9, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, 85m, 8, new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), true, 95m, 8, new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), true, 110m, 9, new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 13, 0, 0, 0, 0, DateTimeKind.Utc), true, 120m, 9, new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 7, 0, 0, 0, 0, DateTimeKind.Utc), true, 95m, 10, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 105m, 10, new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), true, 95m, 11, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), true, 105m, 11, new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, 80m, 12, new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), true, 90m, 12, new DateTime(2026, 5, 9, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, 105m, 13, new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), true, 115m, 13, new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), true, 90m, 14, new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 28, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 13, 0, 0, 0, 0, DateTimeKind.Utc), true, 100m, 14, new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 29, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 7, 0, 0, 0, 0, DateTimeKind.Utc), true, 115m, 15, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 30, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 125m, 15, new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 31, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), true, 75m, 16, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 32, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), true, 85m, 16, new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 33, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, 100m, 17, new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 34, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), true, 110m, 17, new DateTime(2026, 5, 9, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 35, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, 85m, 18, new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 36, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), true, 95m, 18, new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 37, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), true, 110m, 19, new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 38, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 13, 0, 0, 0, 0, DateTimeKind.Utc), true, 120m, 19, new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 39, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 7, 0, 0, 0, 0, DateTimeKind.Utc), true, 95m, 20, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 40, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 105m, 20, new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 41, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), true, 95m, 21, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 42, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), true, 105m, 21, new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 43, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, 80m, 22, new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 44, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), true, 90m, 22, new DateTime(2026, 5, 9, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 45, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, 105m, 23, new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 46, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), true, 115m, 23, new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 47, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), true, 90m, 24, new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 48, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 13, 0, 0, 0, 0, DateTimeKind.Utc), true, 100m, 24, new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 49, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 7, 0, 0, 0, 0, DateTimeKind.Utc), true, 115m, 25, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 125m, 25, new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 51, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), true, 75m, 26, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 52, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), true, 85m, 26, new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 53, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, 100m, 27, new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 54, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), true, 110m, 27, new DateTime(2026, 5, 9, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 55, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, 85m, 28, new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 56, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), true, 95m, 28, new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 57, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), true, 110m, 29, new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 58, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 13, 0, 0, 0, 0, DateTimeKind.Utc), true, 120m, 29, new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 59, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 7, 0, 0, 0, 0, DateTimeKind.Utc), true, 95m, 30, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 60, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 105m, 30, new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 61, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), true, 95m, 31, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 62, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), true, 105m, 31, new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 63, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, 80m, 32, new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 64, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), true, 90m, 32, new DateTime(2026, 5, 9, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 65, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, 105m, 33, new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 66, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), true, 115m, 33, new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 67, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), true, 90m, 34, new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 68, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 13, 0, 0, 0, 0, DateTimeKind.Utc), true, 100m, 34, new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 69, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 7, 0, 0, 0, 0, DateTimeKind.Utc), true, 115m, 35, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 70, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 125m, 35, new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 71, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), true, 75m, 36, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 72, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), true, 85m, 36, new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 73, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, 100m, 37, new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 74, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), true, 110m, 37, new DateTime(2026, 5, 9, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 75, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, 85m, 38, new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 76, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), true, 95m, 38, new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 77, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), true, 110m, 39, new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 78, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 13, 0, 0, 0, 0, DateTimeKind.Utc), true, 120m, 39, new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 79, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 7, 0, 0, 0, 0, DateTimeKind.Utc), true, 95m, 40, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 80, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 105m, 40, new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 81, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), true, 95m, 41, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 82, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), true, 105m, 41, new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 83, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, 80m, 42, new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 84, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), true, 90m, 42, new DateTime(2026, 5, 9, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 85, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, 105m, 43, new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 86, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), true, 115m, 43, new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 87, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), true, 90m, 44, new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 88, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 13, 0, 0, 0, 0, DateTimeKind.Utc), true, 100m, 44, new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 89, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 7, 0, 0, 0, 0, DateTimeKind.Utc), true, 115m, 45, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 90, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 125m, 45, new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 91, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), true, 75m, 46, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 92, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), true, 85m, 46, new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 93, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, 100m, 47, new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 94, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), true, 110m, 47, new DateTime(2026, 5, 9, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 95, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, 85m, 48, new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 96, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), true, 95m, 48, new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 97, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), true, 110m, 49, new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 98, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 13, 0, 0, 0, 0, DateTimeKind.Utc), true, 120m, 49, new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 99, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 7, 0, 0, 0, 0, DateTimeKind.Utc), true, 95m, 50, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 100, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 105m, 50, new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Payment",
                columns: new[] { "Id", "Amount", "BookingId", "CreatedAt", "Method", "Note", "PaidAt", "Status", "TransactionCode" },
                values: new object[,]
                {
                    { 1, 80.0m, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "card", "Deposit payment", new DateTime(2026, 5, 13, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0001" },
                    { 2, 27.0m, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bank_transfer", "Deposit payment", new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0002" },
                    { 3, 63.0m, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bank_transfer", "Final payment", new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0003" },
                    { 4, 112.5m, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "card", "Deposit payment", new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0004" },
                    { 5, 51.0m, 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bank_transfer", "Deposit payment", new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0005" },
                    { 6, 119.0m, 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bank_transfer", "Final payment", new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0006" },
                    { 7, 47.5m, 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "card", "Deposit payment", new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0007" },
                    { 8, 72.0m, 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bank_transfer", "Deposit payment", new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0008" },
                    { 9, 168.0m, 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bank_transfer", "Final payment", new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0009" },
                    { 10, 90.0m, 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "card", "Deposit payment", new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0010" },
                    { 11, 22.5m, 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bank_transfer", "Deposit payment", new DateTime(2026, 5, 9, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0011" },
                    { 12, 52.5m, 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bank_transfer", "Final payment", new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0012" },
                    { 13, 127.5m, 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "card", "Deposit payment", new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0013" },
                    { 14, 57.0m, 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bank_transfer", "Deposit payment", new DateTime(2026, 5, 13, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0014" },
                    { 15, 133.0m, 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bank_transfer", "Final payment", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0015" },
                    { 16, 40.0m, 22, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "card", "Deposit payment", new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0016" },
                    { 17, 81.0m, 24, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bank_transfer", "Deposit payment", new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0017" },
                    { 18, 189.0m, 24, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bank_transfer", "Final payment", new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0018" },
                    { 19, 75.0m, 26, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "card", "Deposit payment", new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0019" },
                    { 20, 25.5m, 28, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bank_transfer", "Deposit payment", new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0020" },
                    { 21, 59.5m, 28, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bank_transfer", "Final payment", new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0021" },
                    { 22, 142.5m, 30, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "card", "Deposit payment", new DateTime(2026, 5, 13, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, "TXN-2026-0022" }
                });

            migrationBuilder.InsertData(
                table: "Review",
                columns: new[] { "Id", "BookingId", "Comment", "CreatedAt", "CustomerId", "Rating", "RoomId" },
                values: new object[,]
                {
                    { 1, 4, "Seeded review for testing", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), 11, (byte)4, 4 },
                    { 2, 8, "Seeded review for testing", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), 15, (byte)5, 8 },
                    { 3, 12, "Seeded review for testing", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), 19, (byte)3, 12 },
                    { 4, 16, "Seeded review for testing", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), 23, (byte)4, 16 },
                    { 5, 20, "Seeded review for testing", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), 27, (byte)5, 20 },
                    { 6, 24, "Seeded review for testing", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), 11, (byte)3, 24 },
                    { 7, 28, "Seeded review for testing", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), 15, (byte)4, 28 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_CancelledBy",
                table: "Booking",
                column: "CancelledBy");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_CheckInDate",
                table: "Booking",
                column: "CheckInDate");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_CheckOutDate",
                table: "Booking",
                column: "CheckOutDate");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_CustomerId",
                table: "Booking",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_RoomId",
                table: "Booking",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Status",
                table: "Booking",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteHotel_HotelId",
                table: "FavoriteHotel",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteHotel_UserId_HotelId",
                table: "FavoriteHotel",
                columns: new[] { "UserId", "HotelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FcmToken_Token",
                table: "FcmToken",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FcmToken_UserId",
                table: "FcmToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotel_IsDeleted",
                table: "Hotel",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Hotel_OwnerId",
                table: "Hotel",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotel_WardId",
                table: "Hotel",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelImage_HotelId",
                table: "HotelImage",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_IsRead",
                table: "Notification",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_SenderId",
                table: "Notification",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId",
                table: "Notification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerSetting_OwnerId",
                table: "OwnerSetting",
                column: "OwnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_BookingId",
                table: "Payment",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_TransactionCode",
                table: "Payment",
                column: "TransactionCode",
                unique: true,
                filter: "[TransactionCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_PermissionKey",
                table: "Permission",
                column: "PermissionKey",
                unique: true,
                filter: "[PermissionKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Province_Code",
                table: "Province",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Province_Name",
                table: "Province",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_IsRevoked",
                table: "RefreshToken",
                column: "IsRevoked");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_Token",
                table: "RefreshToken",
                column: "Token",
                unique: true,
                filter: "[Token] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_BookingId",
                table: "Review",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_CustomerId",
                table: "Review",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_RoomId",
                table: "Review",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                table: "RolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_HotelId",
                table: "Room",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_HotelId_RoomNumber",
                table: "Room",
                columns: new[] { "HotelId", "RoomNumber" },
                unique: true,
                filter: "[RoomNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Room_RoomTypeId",
                table: "Room",
                column: "RoomTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_Status",
                table: "Room",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SystemConfig_ConfigKey",
                table: "SystemConfig",
                column: "ConfigKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlot_RoomId",
                table: "TimeSlot",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlot_RoomId_StartDate_EndDate",
                table: "TimeSlot",
                columns: new[] { "RoomId", "StartDate", "EndDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_DeletedBy",
                table: "User",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Ward_Code",
                table: "Ward",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Ward_ProvinceId_Name",
                table: "Ward",
                columns: new[] { "ProvinceId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteHotel");

            migrationBuilder.DropTable(
                name: "FcmToken");

            migrationBuilder.DropTable(
                name: "HotelImage");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "OwnerSetting");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "SystemConfig");

            migrationBuilder.DropTable(
                name: "TimeSlot");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Hotel");

            migrationBuilder.DropTable(
                name: "RoomType");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Ward");

            migrationBuilder.DropTable(
                name: "Province");
        }
    }
}
