using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trasformazioni.Migrations
{
    /// <inheritdoc />
    public partial class AddFixAuditProblemDocumentiRichiestiLotto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LottoDocumentiRichiesti_LottoId_TipoDocumentoId",
                table: "LottoDocumentiRichiesti");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LottoDocumentiRichiesti");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LottoDocumentiRichiesti");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "LottoDocumentiRichiesti");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "LottoDocumentiRichiesti");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "LottoDocumentiRichiesti");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "LottoDocumentiRichiesti");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "LottoDocumentiRichiesti");

            migrationBuilder.CreateIndex(
                name: "IX_LottoDocumentiRichiesti_LottoId_TipoDocumentoId",
                table: "LottoDocumentiRichiesti",
                columns: new[] { "LottoId", "TipoDocumentoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LottoDocumentiRichiesti_LottoId_TipoDocumentoId",
                table: "LottoDocumentiRichiesti");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LottoDocumentiRichiesti",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "LottoDocumentiRichiesti",
                type: "character varying(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "LottoDocumentiRichiesti",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "LottoDocumentiRichiesti",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "LottoDocumentiRichiesti",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "LottoDocumentiRichiesti",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "LottoDocumentiRichiesti",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LottoDocumentiRichiesti_LottoId_TipoDocumentoId",
                table: "LottoDocumentiRichiesti",
                columns: new[] { "LottoId", "TipoDocumentoId" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }
    }
}
