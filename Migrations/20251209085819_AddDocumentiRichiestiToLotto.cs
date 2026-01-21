using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trasformazioni.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentiRichiestiToLotto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LottoDocumentiRichiesti",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LottoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoDocumentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LottoDocumentiRichiesti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LottoDocumentiRichiesti_Lotti_LottoId",
                        column: x => x.LottoId,
                        principalTable: "Lotti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LottoDocumentiRichiesti_TipiDocumento_TipoDocumentoId",
                        column: x => x.TipoDocumentoId,
                        principalTable: "TipiDocumento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LottoDocumentiRichiesti_LottoId",
                table: "LottoDocumentiRichiesti",
                column: "LottoId");

            migrationBuilder.CreateIndex(
                name: "IX_LottoDocumentiRichiesti_LottoId_TipoDocumentoId",
                table: "LottoDocumentiRichiesti",
                columns: new[] { "LottoId", "TipoDocumentoId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_LottoDocumentiRichiesti_TipoDocumentoId",
                table: "LottoDocumentiRichiesti",
                column: "TipoDocumentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LottoDocumentiRichiesti");
        }
    }
}
