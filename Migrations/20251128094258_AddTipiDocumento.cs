using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trasformazioni.Migrations
{
    /// <inheritdoc />
    public partial class AddTipiDocumento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TipoDocumentoId",
                table: "DocumentiGara",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "TipiDocumento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Area = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipiDocumento", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_TipoDocumentoId",
                table: "DocumentiGara",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_TipiDocumento_Area",
                table: "TipiDocumento",
                column: "Area");

            migrationBuilder.CreateIndex(
                name: "IX_TipiDocumento_Area_IsDeleted",
                table: "TipiDocumento",
                columns: new[] { "Area", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_TipiDocumento_CreatedAt",
                table: "TipiDocumento",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TipiDocumento_IsDeleted",
                table: "TipiDocumento",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TipiDocumento_IsSystem",
                table: "TipiDocumento",
                column: "IsSystem");

            migrationBuilder.CreateIndex(
                name: "IX_TipiDocumento_Nome_Area",
                table: "TipiDocumento",
                columns: new[] { "Nome", "Area" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentiGara_TipiDocumento_TipoDocumentoId",
                table: "DocumentiGara",
                column: "TipoDocumentoId",
                principalTable: "TipiDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentiGara_TipiDocumento_TipoDocumentoId",
                table: "DocumentiGara");

            migrationBuilder.DropTable(
                name: "TipiDocumento");

            migrationBuilder.DropIndex(
                name: "IX_DocumentiGara_TipoDocumentoId",
                table: "DocumentiGara");

            migrationBuilder.DropColumn(
                name: "TipoDocumentoId",
                table: "DocumentiGara");
        }
    }
}
