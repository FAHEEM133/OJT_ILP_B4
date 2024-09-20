using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initialcommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Markets",
                columns: table => new
                {
                    MarketId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MarketCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    LongMarketCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CountryList = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markets", x => x.MarketId);
                });

            migrationBuilder.CreateTable(
                name: "MarketSubGroups",
                columns: table => new
                {
                    SubGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubGroupName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SubGroupCode = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    MarketId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketSubGroups", x => x.SubGroupId);
                    table.ForeignKey(
                        name: "FK_MarketSubGroups_Markets_MarketId",
                        column: x => x.MarketId,
                        principalTable: "Markets",
                        principalColumn: "MarketId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Markets_MarketCode",
                table: "Markets",
                column: "MarketCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Markets_MarketName",
                table: "Markets",
                column: "MarketName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketSubGroups_MarketId_SubGroupCode",
                table: "MarketSubGroups",
                columns: new[] { "MarketId", "SubGroupCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketSubGroups_MarketId_SubGroupName",
                table: "MarketSubGroups",
                columns: new[] { "MarketId", "SubGroupName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketSubGroups");

            migrationBuilder.DropTable(
                name: "Markets");
        }
    }
}
