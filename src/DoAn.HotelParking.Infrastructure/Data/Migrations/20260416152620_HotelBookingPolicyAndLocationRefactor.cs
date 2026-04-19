using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAn.HotelParking.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class HotelBookingPolicyAndLocationRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_User_ApprovedBy",
                table: "Booking");

            migrationBuilder.Sql("IF OBJECT_ID(N'[LicensePlateLog]', N'U') IS NOT NULL DROP TABLE [LicensePlateLog];");

            migrationBuilder.Sql("IF OBJECT_ID(N'[RoomImage]', N'U') IS NOT NULL DROP TABLE [RoomImage];");

            migrationBuilder.Sql("IF OBJECT_ID(N'[ParkingSession]', N'U') IS NOT NULL DROP TABLE [ParkingSession];");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Booking_ApprovedBy",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_CreatedAt",
                table: "Booking");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_DepositAmount",
                table: "Booking");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_DepositLessThanTotal",
                table: "Booking");

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 9, 3 });

            migrationBuilder.DeleteData(
                table: "RoomType",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Hotel");

            migrationBuilder.DropColumn(
                name: "Ward",
                table: "Hotel");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "PaymentProofUrl",
                table: "Booking");

            migrationBuilder.RenameColumn(
                name: "DepositAmount",
                table: "Booking",
                newName: "RoomUnitPrice");

            migrationBuilder.Sql(@"IF COL_LENGTH('Payment', 'Id') IS NULL ALTER TABLE [Payment] ADD [Id] int IDENTITY(1,1) NOT NULL;");

            migrationBuilder.Sql(@"IF COL_LENGTH('Payment', 'Note') IS NULL ALTER TABLE [Payment] ADD [Note] nvarchar(255) NULL;");

            migrationBuilder.Sql(@"IF COL_LENGTH('Payment', 'PaidAt') IS NULL ALTER TABLE [Payment] ADD [PaidAt] datetime2 NULL;");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "Hotel",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.Sql(@"IF COL_LENGTH('Hotel', 'OwnerId') IS NULL ALTER TABLE [Hotel] ADD [OwnerId] int NOT NULL DEFAULT 0;");

            migrationBuilder.Sql(@"IF COL_LENGTH('Hotel', 'UpdatedAt') IS NULL ALTER TABLE [Hotel] ADD [UpdatedAt] datetime2 NULL DEFAULT (SYSUTCDATETIME());");

            migrationBuilder.Sql(@"IF COL_LENGTH('Hotel', 'WardId') IS NULL ALTER TABLE [Hotel] ADD [WardId] int NOT NULL DEFAULT 0;");

            migrationBuilder.Sql(@"IF COL_LENGTH('Booking', 'CancelReason') IS NULL ALTER TABLE [Booking] ADD [CancelReason] nvarchar(255) NULL;");

            migrationBuilder.Sql(@"IF COL_LENGTH('Booking', 'CheckInDate') IS NULL ALTER TABLE [Booking] ADD [CheckInDate] datetime2 NOT NULL DEFAULT ('0001-01-01T00:00:00.0000000');");

            migrationBuilder.Sql(@"IF COL_LENGTH('Booking', 'CheckOutDate') IS NULL ALTER TABLE [Booking] ADD [CheckOutDate] datetime2 NOT NULL DEFAULT ('0001-01-01T00:00:00.0000000');");

            migrationBuilder.Sql(@"IF COL_LENGTH('Booking', 'GuestCount') IS NULL ALTER TABLE [Booking] ADD [GuestCount] int NOT NULL DEFAULT 0;");

            migrationBuilder.Sql(@"IF COL_LENGTH('Booking', 'NightCount') IS NULL ALTER TABLE [Booking] ADD [NightCount] int NOT NULL DEFAULT 0;");

            migrationBuilder.Sql(@"IF COL_LENGTH('Booking', 'PaidAmount') IS NULL ALTER TABLE [Booking] ADD [PaidAmount] decimal(10,2) NOT NULL DEFAULT 0;");

            migrationBuilder.Sql(@"IF COL_LENGTH('Booking', 'TimeSlotId') IS NULL ALTER TABLE [Booking] ADD [TimeSlotId] int NOT NULL DEFAULT 0;");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "Id");

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
                name: "TimeSlot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CheckInFrom = table.Column<TimeOnly>(type: "time", nullable: false),
                    CheckOutUntil = table.Column<TimeOnly>(type: "time", nullable: false),
                    CancellationHoursBeforeCheckIn = table.Column<int>(type: "int", nullable: false),
                    MinStayNights = table.Column<int>(type: "int", nullable: false),
                    MaxStayNights = table.Column<int>(type: "int", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlot", x => x.Id);
                    table.CheckConstraint("CK_TimeSlot_CancellationHours", "[CancellationHoursBeforeCheckIn] >= 0");
                    table.CheckConstraint("CK_TimeSlot_MaxStayNights", "[MaxStayNights] IS NULL OR [MaxStayNights] >= [MinStayNights]");
                    table.CheckConstraint("CK_TimeSlot_MinStayNights", "[MinStayNights] >= 1");
                    table.ForeignKey(
                        name: "FK_TimeSlot_Hotel_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "District",
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
                    table.PrimaryKey("PK_District", x => x.Id);
                    table.ForeignKey(
                        name: "FK_District_Province_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Province",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ward",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ward", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ward_District_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "District",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Module", "PermissionKey" },
                values: new object[] { "Read hotels", "Hotel", "hotel.read" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Module", "PermissionKey" },
                values: new object[] { "Manage hotels", "Hotel", "hotel.manage" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Module", "PermissionKey" },
                values: new object[] { "Read rooms", "Room", "room.read" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "Module", "PermissionKey" },
                values: new object[] { "Manage rooms", "Room", "room.manage" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Description", "Module", "PermissionKey" },
                values: new object[] { "Read bookings", "Booking", "booking.read" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "Module", "PermissionKey" },
                values: new object[] { "Manage bookings", "Booking", "booking.manage" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Description", "Module", "PermissionKey" },
                values: new object[] { "Read payments", "Payment", "payment.read" });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "CreatedAt", "Description", "Module", "PermissionKey", "UpdatedAt" },
                values: new object[,]
                {
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage payments", "Payment", "payment.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read reviews", "Review", "review.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage reviews", "Review", "review.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read notifications", "Notification", "notification.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read locations", "Location", "location.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage locations", "Location", "location.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read time slots", "TimeSlot", "timeslot.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage time slots", "TimeSlot", "timeslot.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Read hotel images", "HotelImage", "hotelimage.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage hotel images", "HotelImage", "hotelimage.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "PermissionId", "RoleId", "CreatedAt" },
                values: new object[,]
                {
                    { 4, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
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
                    { 11, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_BookingId",
                table: "Payment",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotel_OwnerId",
                table: "Hotel",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotel_WardId",
                table: "Hotel",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_CheckInDate",
                table: "Booking",
                column: "CheckInDate");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_CheckOutDate",
                table: "Booking",
                column: "CheckOutDate");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_TimeSlotId",
                table: "Booking",
                column: "TimeSlotId");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Booking_CheckOutAfterCheckIn')
BEGIN
    ALTER TABLE [Booking] ADD CONSTRAINT [CK_Booking_CheckOutAfterCheckIn] CHECK ([CheckOutDate] > [CheckInDate]);
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Booking_GuestCount')
BEGIN
    ALTER TABLE [Booking] ADD CONSTRAINT [CK_Booking_GuestCount] CHECK ([GuestCount] >= 1);
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Booking_NightCount')
BEGIN
    ALTER TABLE [Booking] ADD CONSTRAINT [CK_Booking_NightCount] CHECK ([NightCount] >= 1);
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Booking_PaidAmount')
BEGIN
    ALTER TABLE [Booking] ADD CONSTRAINT [CK_Booking_PaidAmount] CHECK ([PaidAmount] >= 0);
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Booking_PaidAmountNotOverTotal')
BEGIN
    ALTER TABLE [Booking] ADD CONSTRAINT [CK_Booking_PaidAmountNotOverTotal] CHECK ([PaidAmount] <= [TotalAmount]);
END");

            migrationBuilder.CreateIndex(
                name: "IX_District_Code",
                table: "District",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_District_ProvinceId_Name",
                table: "District",
                columns: new[] { "ProvinceId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HotelImage_HotelId",
                table: "HotelImage",
                column: "HotelId");

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
                name: "IX_TimeSlot_HotelId",
                table: "TimeSlot",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlot_HotelId_IsDefault",
                table: "TimeSlot",
                columns: new[] { "HotelId", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_Ward_Code",
                table: "Ward",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Ward_DistrictId_Name",
                table: "Ward",
                columns: new[] { "DistrictId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_TimeSlot_TimeSlotId",
                table: "Booking",
                column: "TimeSlotId",
                principalTable: "TimeSlot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hotel_User_OwnerId",
                table: "Hotel",
                column: "OwnerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hotel_Ward_WardId",
                table: "Hotel",
                column: "WardId",
                principalTable: "Ward",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_TimeSlot_TimeSlotId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Hotel_User_OwnerId",
                table: "Hotel");

            migrationBuilder.DropForeignKey(
                name: "FK_Hotel_Ward_WardId",
                table: "Hotel");

            migrationBuilder.DropTable(
                name: "HotelImage");

            migrationBuilder.DropTable(
                name: "TimeSlot");

            migrationBuilder.DropTable(
                name: "Ward");

            migrationBuilder.DropTable(
                name: "District");

            migrationBuilder.DropTable(
                name: "Province");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_BookingId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Hotel_OwnerId",
                table: "Hotel");

            migrationBuilder.DropIndex(
                name: "IX_Hotel_WardId",
                table: "Hotel");

            migrationBuilder.DropIndex(
                name: "IX_Booking_CheckInDate",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_CheckOutDate",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_TimeSlotId",
                table: "Booking");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_CheckOutAfterCheckIn",
                table: "Booking");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_GuestCount",
                table: "Booking");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_NightCount",
                table: "Booking");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_PaidAmount",
                table: "Booking");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_PaidAmountNotOverTotal",
                table: "Booking");

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 10, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 11, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 12, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 13, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 14, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 15, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 16, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 17, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 18, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 19, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 11, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 13, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 14, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 16, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 17, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 18, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 19, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 7, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 8, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 11, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 12, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 13, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 14, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 16, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 18, 3 });

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Hotel");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Hotel");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "Hotel");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "CheckInDate",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "CheckOutDate",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "GuestCount",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "NightCount",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TimeSlotId",
                table: "Booking");

            migrationBuilder.RenameColumn(
                name: "RoomUnitPrice",
                table: "Booking",
                newName: "DepositAmount");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "Hotel",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Hotel",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ward",
                table: "Hotel",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Booking",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                table: "Booking",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentProofUrl",
                table: "Booking",
                type: "varchar(max)",
                unicode: false,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "BookingId");

            migrationBuilder.CreateTable(
                name: "ParkingSession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QrUserId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    VerifiedBy = table.Column<int>(type: "int", nullable: true),
                    CheckInPlate = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    CheckInTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutPlate = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    CheckOutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    Status = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParkingSession_User_QrUserId",
                        column: x => x.QrUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParkingSession_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParkingSession_User_VerifiedBy",
                        column: x => x.VerifiedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RoomImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    IsMain = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomImage_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LicensePlateLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParkingSessionId = table.Column<int>(type: "int", nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    DetectedPlate = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    ImageUrl = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicensePlateLog", x => x.Id);
                    table.CheckConstraint("CK_LicensePlateLog_Confidence", "[Confidence] >= 0 AND [Confidence] <= 1");
                    table.ForeignKey(
                        name: "FK_LicensePlateLog_ParkingSession_ParkingSessionId",
                        column: x => x.ParkingSessionId,
                        principalTable: "ParkingSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Module", "PermissionKey" },
                values: new object[] { "Read rooms", "Room", "room.read" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Module", "PermissionKey" },
                values: new object[] { "Manage rooms", "Room", "room.manage" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Module", "PermissionKey" },
                values: new object[] { "Read bookings", "Booking", "booking.read" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "Module", "PermissionKey" },
                values: new object[] { "Manage bookings", "Booking", "booking.manage" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Description", "Module", "PermissionKey" },
                values: new object[] { "Read parking sessions", "Parking", "parking.read" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "Module", "PermissionKey" },
                values: new object[] { "Manage parking sessions", "Parking", "parking.manage" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Description", "Module", "PermissionKey" },
                values: new object[] { "Read notifications", "Notification", "notification.read" });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "PermissionId", "RoleId", "CreatedAt" },
                values: new object[] { 9, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "RoomType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 3, "Suite room", "Suite" });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_ApprovedBy",
                table: "Booking",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_CreatedAt",
                table: "Booking",
                column: "CreatedAt");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Booking_DepositAmount",
                table: "Booking",
                sql: "[DepositAmount] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Booking_DepositLessThanTotal",
                table: "Booking",
                sql: "[DepositAmount] <= [TotalAmount]");

            migrationBuilder.CreateIndex(
                name: "IX_LicensePlateLog_ParkingSessionId_CreatedAt",
                table: "LicensePlateLog",
                columns: new[] { "ParkingSessionId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSession_QrUserId",
                table: "ParkingSession",
                column: "QrUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSession_Status",
                table: "ParkingSession",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSession_UserId",
                table: "ParkingSession",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSession_VerifiedBy",
                table: "ParkingSession",
                column: "VerifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RoomImage_RoomId",
                table: "RoomImage",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_User_ApprovedBy",
                table: "Booking",
                column: "ApprovedBy",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
