using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trasformazioni.Migrations
{
    /// <inheritdoc />
    public partial class AddGaraDocumentoRichiestoChecklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GaraDocumentiRichiesti",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GaraId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoDocumentoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GaraDocumentiRichiesti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GaraDocumentiRichiesti_Gare_GaraId",
                        column: x => x.GaraId,
                        principalTable: "Gare",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GaraDocumentiRichiesti_TipiDocumento_TipoDocumentoId",
                        column: x => x.TipoDocumentoId,
                        principalTable: "TipiDocumento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GaraDocumentiRichiesti_GaraId",
                table: "GaraDocumentiRichiesti",
                column: "GaraId");

            migrationBuilder.CreateIndex(
                name: "IX_GaraDocumentiRichiesti_GaraId_TipoDocumentoId",
                table: "GaraDocumentiRichiesti",
                columns: new[] { "GaraId", "TipoDocumentoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GaraDocumentiRichiesti_TipoDocumentoId",
                table: "GaraDocumentiRichiesti",
                column: "TipoDocumentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GaraDocumentiRichiesti");
        }
    }
}
