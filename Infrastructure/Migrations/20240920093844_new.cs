using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryList",
                table: "Markets");

            migrationBuilder.RenameColumn(
                name: "MarketId",
                table: "Markets",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Markets",
                newName: "MarketId");

            migrationBuilder.AddColumn<string>(
                name: "CountryList",
                table: "Markets",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
