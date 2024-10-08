using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
<<<<<<<< HEAD:Infrastructure/Migrations/20241003090752_Migration-Initial.cs
    public partial class MigrationInitial : Migration
========
    public partial class SubgroupTableUpdatev1 : Migration
>>>>>>>> dev:Infrastructure/Migrations/20241001064354_SubgroupTableUpdatev1.cs
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Markets",
                columns: table => new
                {
<<<<<<<< HEAD:Infrastructure/Migrations/20241003090752_Migration-Initial.cs
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    LongMarketCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Region = table.Column<int>(type: "integer", nullable: false),
                    SubRegion = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
========
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    LongMarketCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Region = table.Column<int>(type: "int", nullable: false),
                    SubRegion = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
>>>>>>>> dev:Infrastructure/Migrations/20241001064354_SubgroupTableUpdatev1.cs
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markets", x => x.Id);
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Markets_Code",
                table: "Markets",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Markets_Name",
                table: "Markets",
                column: "Name",
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
