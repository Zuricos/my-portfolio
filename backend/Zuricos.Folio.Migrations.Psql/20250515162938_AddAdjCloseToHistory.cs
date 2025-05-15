using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zuricos.Folio.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAdjCloseToHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AdjustedClose",
                table: "AssetHistories",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdjustedClose",
                table: "AssetHistories");
        }
    }
}
