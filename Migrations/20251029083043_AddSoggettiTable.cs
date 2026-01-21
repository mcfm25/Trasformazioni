using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trasformazioni.Migrations
{
    /// <inheritdoc />
    public partial class AddSoggettiTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Soggetti",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodiceInterno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TipoSoggetto = table.Column<int>(type: "integer", nullable: false),
                    NaturaGiuridica = table.Column<int>(type: "integer", nullable: false),
                    IsCliente = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsFornitore = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Denominazione = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Cognome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CodiceFiscale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    PartitaIVA = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CodiceSDI = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    Referente = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PEC = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TipoVia = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NomeVia = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    NumeroCivico = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Citta = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CAP = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Provincia = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Nazione = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CondizioniPagamento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IBAN = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: true),
                    ScontoPartner = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_Soggetti", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Soggetti_CodiceFiscale",
                table: "Soggetti",
                column: "CodiceFiscale");

            migrationBuilder.CreateIndex(
                name: "IX_Soggetti_CodiceInterno",
                table: "Soggetti",
                column: "CodiceInterno",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Soggetti_CreatedAt",
                table: "Soggetti",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Soggetti_Email",
                table: "Soggetti",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Soggetti_IsDeleted",
                table: "Soggetti",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Soggetti_PartitaIVA",
                table: "Soggetti",
                column: "PartitaIVA");

            migrationBuilder.CreateIndex(
                name: "IX_Soggetti_TipoSoggetto_IsCliente_IsFornitore",
                table: "Soggetti",
                columns: new[] { "TipoSoggetto", "IsCliente", "IsFornitore" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Soggetti");
        }
    }
}
