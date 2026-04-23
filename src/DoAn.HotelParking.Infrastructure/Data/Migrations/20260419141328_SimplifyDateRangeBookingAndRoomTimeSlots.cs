using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn.HotelParking.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyDateRangeBookingAndRoomTimeSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_TimeSlot_TimeSlotId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlot_Hotel_HotelId",
                table: "TimeSlot");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlot_HotelId",
                table: "TimeSlot");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlot_HotelId_IsDefault",
                table: "TimeSlot");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TimeSlot_CancellationHours",
                table: "TimeSlot");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TimeSlot_MaxStayNights",
                table: "TimeSlot");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TimeSlot_MinStayNights",
                table: "TimeSlot");

            migrationBuilder.DropIndex(
                name: "IX_Booking_TimeSlotId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "CancellationHoursBeforeCheckIn",
                table: "TimeSlot");

            migrationBuilder.DropColumn(
                name: "CheckInFrom",
                table: "TimeSlot");

            migrationBuilder.DropColumn(
                name: "CheckOutUntil",
                table: "TimeSlot");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "TimeSlot");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "TimeSlot");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TimeSlot");

            migrationBuilder.DropColumn(
                name: "MaxStayNights",
                table: "TimeSlot");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "TimeSlot");

            migrationBuilder.DropColumn(
                name: "TimeSlotId",
                table: "Booking");

            migrationBuilder.RenameColumn(
                name: "MinStayNights",
                table: "TimeSlot",
                newName: "RoomId");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "TimeSlot",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TimeSlot",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "TimeSlot",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "TimeSlot",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TimeSlot",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "SYSUTCDATETIME()");

            // Legacy policy-based rows are incompatible with the new room/date-range model.
            migrationBuilder.Sql("DELETE FROM [TimeSlot];");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlot_RoomId",
                table: "TimeSlot",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlot_RoomId_StartDate_EndDate",
                table: "TimeSlot",
                columns: new[] { "RoomId", "StartDate", "EndDate" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_TimeSlot_DateRange",
                table: "TimeSlot",
                sql: "[EndDate] > [StartDate]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TimeSlot_Price",
                table: "TimeSlot",
                sql: "[Price] >= 0");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlot_Room_RoomId",
                table: "TimeSlot",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlot_Room_RoomId",
                table: "TimeSlot");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlot_RoomId",
                table: "TimeSlot");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlot_RoomId_StartDate_EndDate",
                table: "TimeSlot");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TimeSlot_DateRange",
                table: "TimeSlot");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TimeSlot_Price",
                table: "TimeSlot");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "TimeSlot");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TimeSlot");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "TimeSlot");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "TimeSlot");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TimeSlot");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "TimeSlot",
                newName: "MinStayNights");

            migrationBuilder.AddColumn<int>(
                name: "CancellationHoursBeforeCheckIn",
                table: "TimeSlot",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "CheckInFrom",
                table: "TimeSlot",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "CheckOutUntil",
                table: "TimeSlot",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "HotelId",
                table: "TimeSlot",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "TimeSlot",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TimeSlot",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxStayNights",
                table: "TimeSlot",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TimeSlot",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TimeSlotId",
                table: "Booking",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlot_HotelId",
                table: "TimeSlot",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlot_HotelId_IsDefault",
                table: "TimeSlot",
                columns: new[] { "HotelId", "IsDefault" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_TimeSlot_CancellationHours",
                table: "TimeSlot",
                sql: "[CancellationHoursBeforeCheckIn] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TimeSlot_MaxStayNights",
                table: "TimeSlot",
                sql: "[MaxStayNights] IS NULL OR [MaxStayNights] >= [MinStayNights]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TimeSlot_MinStayNights",
                table: "TimeSlot",
                sql: "[MinStayNights] >= 1");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_TimeSlotId",
                table: "Booking",
                column: "TimeSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_TimeSlot_TimeSlotId",
                table: "Booking",
                column: "TimeSlotId",
                principalTable: "TimeSlot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlot_Hotel_HotelId",
                table: "TimeSlot",
                column: "HotelId",
                principalTable: "Hotel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
