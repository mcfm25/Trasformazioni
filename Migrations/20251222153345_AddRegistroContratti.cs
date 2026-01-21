using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trasformazioni.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistroContratti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategorieContratto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Ordine = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsAttivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
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
                    table.PrimaryKey("PK_CategorieContratto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistroContratti",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdRiferimentoEsterno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NumeroProtocollo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TipoRegistro = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: true),
                    RagioneSociale = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TipoControparte = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Oggetto = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CategoriaContrattoId = table.Column<Guid>(type: "uuid", nullable: false),
                    UtenteId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    ResponsabileInterno = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DataDocumento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataDecorrenza = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataFineOScadenza = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataInvio = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataAccettazione = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GiorniPreavvisoDisdetta = table.Column<int>(type: "integer", nullable: true),
                    GiorniAlertScadenza = table.Column<int>(type: "integer", nullable: false, defaultValue: 60),
                    IsRinnovoAutomatico = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    GiorniRinnovoAutomatico = table.Column<int>(type: "integer", nullable: true),
                    ImportoCanoneAnnuo = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ImportoUnatantum = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Stato = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_RegistroContratti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistroContratti_AspNetUsers_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistroContratti_CategorieContratto_CategoriaContrattoId",
                        column: x => x.CategoriaContrattoId,
                        principalTable: "CategorieContratto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistroContratti_RegistroContratti_ParentId",
                        column: x => x.ParentId,
                        principalTable: "RegistroContratti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistroContratti_Soggetti_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Soggetti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AllegatiRegistro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegistroContrattiId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoDocumentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NomeFile = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PathMinIO = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DimensioneBytes = table.Column<long>(type: "bigint", nullable: false),
                    MimeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsUploadCompleto = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DataCaricamento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CaricatoDaUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
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
                    table.PrimaryKey("PK_AllegatiRegistro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllegatiRegistro_AspNetUsers_CaricatoDaUserId",
                        column: x => x.CaricatoDaUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllegatiRegistro_RegistroContratti_RegistroContrattiId",
                        column: x => x.RegistroContrattiId,
                        principalTable: "RegistroContratti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllegatiRegistro_TipiDocumento_TipoDocumentoId",
                        column: x => x.TipoDocumentoId,
                        principalTable: "TipiDocumento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllegatiRegistro_CaricatoDaUserId",
                table: "AllegatiRegistro",
                column: "CaricatoDaUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AllegatiRegistro_CreatedAt",
                table: "AllegatiRegistro",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AllegatiRegistro_DataCaricamento",
                table: "AllegatiRegistro",
                column: "DataCaricamento");

            migrationBuilder.CreateIndex(
                name: "IX_AllegatiRegistro_IsDeleted",
                table: "AllegatiRegistro",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AllegatiRegistro_IsUploadCompleto",
                table: "AllegatiRegistro",
                column: "IsUploadCompleto");

            migrationBuilder.CreateIndex(
                name: "IX_AllegatiRegistro_RegistroContrattiId",
                table: "AllegatiRegistro",
                column: "RegistroContrattiId");

            migrationBuilder.CreateIndex(
                name: "IX_AllegatiRegistro_TipoDocumentoId",
                table: "AllegatiRegistro",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorieContratto_CreatedAt",
                table: "CategorieContratto",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CategorieContratto_IsAttivo",
                table: "CategorieContratto",
                column: "IsAttivo");

            migrationBuilder.CreateIndex(
                name: "IX_CategorieContratto_IsDeleted",
                table: "CategorieContratto",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CategorieContratto_Nome",
                table: "CategorieContratto",
                column: "Nome",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_CategorieContratto_Ordine",
                table: "CategorieContratto",
                column: "Ordine");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroContratti_CategoriaContrattoId",
                table: "RegistroContratti",
                column: "CategoriaContrattoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroContratti_ClienteId_Stato",
                table: "RegistroContratti",
                columns: new[] { "ClienteId", "Stato" });

            migrationBuilder.CreateIndex(
                name: "IX_RegistroContratti_CreatedAt",
                table: "RegistroContratti",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroContratti_DataFineOScadenza",
                table: "RegistroContratti",
                column: "DataFineOScadenza");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroContratti_IdRiferimentoEsterno",
                table: "RegistroContratti",
                column: "IdRiferimentoEsterno");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroContratti_IsDeleted",
                table: "RegistroContratti",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroContratti_NumeroProtocollo",
                table: "RegistroContratti",
                column: "NumeroProtocollo",
                unique: true,
                filter: "\"NumeroProtocollo\" IS NOT NULL AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroContratti_ParentId",
                table: "RegistroContratti",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroContratti_TipoRegistro",
                table: "RegistroContratti",
                column: "TipoRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroContratti_UtenteId",
                table: "RegistroContratti",
                column: "UtenteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllegatiRegistro");

            migrationBuilder.DropTable(
                name: "RegistroContratti");

            migrationBuilder.DropTable(
                name: "CategorieContratto");
        }
    }
}
