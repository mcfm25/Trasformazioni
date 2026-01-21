using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trasformazioni.Migrations
{
    /// <inheritdoc />
    public partial class MigrateDocumentoGaraToTipoDocumentoFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DocumentiGara_GaraId_Tipo",
                table: "DocumentiGara");

            migrationBuilder.DropIndex(
                name: "IX_DocumentiGara_LottoId_Tipo",
                table: "DocumentiGara");

            migrationBuilder.DropIndex(
                name: "IX_DocumentiGara_Tipo",
                table: "DocumentiGara");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "DocumentiGara");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_GaraId_TipoDocumentoId",
                table: "DocumentiGara",
                columns: new[] { "GaraId", "TipoDocumentoId" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_LottoId_TipoDocumentoId",
                table: "DocumentiGara",
                columns: new[] { "LottoId", "TipoDocumentoId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DocumentiGara_GaraId_TipoDocumentoId",
                table: "DocumentiGara");

            migrationBuilder.DropIndex(
                name: "IX_DocumentiGara_LottoId_TipoDocumentoId",
                table: "DocumentiGara");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "DocumentiGara",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_GaraId_Tipo",
                table: "DocumentiGara",
                columns: new[] { "GaraId", "Tipo" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_LottoId_Tipo",
                table: "DocumentiGara",
                columns: new[] { "LottoId", "Tipo" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_Tipo",
                table: "DocumentiGara",
                column: "Tipo");
        }
    }
}
