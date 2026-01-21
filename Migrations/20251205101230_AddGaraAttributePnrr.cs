using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trasformazioni.Migrations
{
    /// <inheritdoc />
    public partial class AddGaraAttributePnrr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PNRR",
                table: "Gare",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PNRR",
                table: "Gare");
        }
    }
}
