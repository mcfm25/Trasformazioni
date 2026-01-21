using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trasformazioni.Migrations
{
    /// <inheritdoc />
    public partial class FixElaborazioneLottoUniqueIndexForSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ElaborazioniLotti_LottoId",
                table: "ElaborazioniLotti");

            migrationBuilder.CreateIndex(
                name: "IX_ElaborazioniLotti_LottoId",
                table: "ElaborazioniLotti",
                column: "LottoId",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ElaborazioniLotti_LottoId",
                table: "ElaborazioniLotti");

            migrationBuilder.CreateIndex(
                name: "IX_ElaborazioniLotti_LottoId",
                table: "ElaborazioniLotti",
                column: "LottoId",
                unique: true);
        }
    }
}
