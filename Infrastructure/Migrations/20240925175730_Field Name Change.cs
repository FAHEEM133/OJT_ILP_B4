using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FieldNameChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MarketName",
                table: "Markets",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "MarketCode",
                table: "Markets",
                newName: "Code");

            migrationBuilder.RenameIndex(
                name: "IX_Markets_MarketName",
                table: "Markets",
                newName: "IX_Markets_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Markets_MarketCode",
                table: "Markets",
                newName: "IX_Markets_Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Markets",
                newName: "MarketName");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Markets",
                newName: "MarketCode");

            migrationBuilder.RenameIndex(
                name: "IX_Markets_Name",
                table: "Markets",
                newName: "IX_Markets_MarketName");

            migrationBuilder.RenameIndex(
                name: "IX_Markets_Code",
                table: "Markets",
                newName: "IX_Markets_MarketCode");
        }
    }
}
