using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn.HotelParking.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomPriceAndOwnerRoleSeedUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Room",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Hotel owner", "Owner" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Room_Price",
                table: "Room",
                sql: "[Price] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Room_Price",
                table: "Room");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Room");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Hotel staff", "Staff" });
        }
    }
}
