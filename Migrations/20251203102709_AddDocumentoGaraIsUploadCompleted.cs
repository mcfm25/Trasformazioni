using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trasformazioni.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentoGaraIsUploadCompleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUploadCompleto",
                table: "DocumentiGara",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_IsUploadCompleto",
                table: "DocumentiGara",
                column: "IsUploadCompleto",
                filter: "\"IsUploadCompleto\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DocumentiGara_IsUploadCompleto",
                table: "DocumentiGara");

            migrationBuilder.DropColumn(
                name: "IsUploadCompleto",
                table: "DocumentiGara");
        }
    }
}
