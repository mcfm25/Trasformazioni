using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trasformazioni.Migrations
{
    /// <inheritdoc />
    public partial class AddCodiceRiferimentoInTipiDocumento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodiceRiferimento",
                table: "TipiDocumento",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TipiDocumento_CodiceRiferimento",
                table: "TipiDocumento",
                column: "CodiceRiferimento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TipiDocumento_CodiceRiferimento",
                table: "TipiDocumento");

            migrationBuilder.DropColumn(
                name: "CodiceRiferimento",
                table: "TipiDocumento");
        }
    }
}
