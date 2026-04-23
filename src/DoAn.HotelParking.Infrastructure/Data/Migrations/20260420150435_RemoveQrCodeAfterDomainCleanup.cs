using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn.HotelParking.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveQrCodeAfterDomainCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // If a temporary 2-state enum version was deployed, normalize polluted status values.
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Booking]', N'U') IS NOT NULL
                BEGIN
                    UPDATE b
                    SET b.[Status] = 2
                    FROM [Booking] AS b
                    WHERE b.[Status] = 1
                      AND (b.[CancelledAt] IS NOT NULL OR b.[CancelReason] IS NOT NULL OR b.[CancelledBy] IS NOT NULL);

                    UPDATE b
                    SET b.[Status] = 1
                    FROM [Booking] AS b
                    WHERE b.[Status] = 0
                      AND b.[PaidAmount] >= b.[TotalAmount]
                      AND b.[CancelledAt] IS NULL
                      AND b.[CancelReason] IS NULL
                      AND b.[CancelledBy] IS NULL;
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[QrCode]', N'U') IS NOT NULL
                BEGIN
                    DROP TABLE [QrCode];
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QrCode",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrCode", x => x.Code);
                    table.ForeignKey(
                        name: "FK_QrCode_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QrCode_UserId",
                table: "QrCode",
                column: "UserId");
        }
    }
}
