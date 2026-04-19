using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn.HotelParking.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDistrictAndRefactorWardProvince : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ward_District_DistrictId",
                table: "Ward");

            migrationBuilder.DropTable(
                name: "District");

            migrationBuilder.RenameColumn(
                name: "DistrictId",
                table: "Ward",
                newName: "ProvinceId");

            migrationBuilder.RenameIndex(
                name: "IX_Ward_DistrictId_Name",
                table: "Ward",
                newName: "IX_Ward_ProvinceId_Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Ward_Province_ProvinceId",
                table: "Ward",
                column: "ProvinceId",
                principalTable: "Province",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ward_Province_ProvinceId",
                table: "Ward");

            migrationBuilder.RenameColumn(
                name: "ProvinceId",
                table: "Ward",
                newName: "DistrictId");

            migrationBuilder.RenameIndex(
                name: "IX_Ward_ProvinceId_Name",
                table: "Ward",
                newName: "IX_Ward_DistrictId_Name");

            migrationBuilder.CreateTable(
                name: "District",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "FK_Ward_District_DistrictId",
                table: "Ward",
                column: "DistrictId",
                principalTable: "District",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
