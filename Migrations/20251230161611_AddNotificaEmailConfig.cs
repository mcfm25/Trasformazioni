using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trasformazioni.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificaEmailConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigurazioniNotificaEmail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codice = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Modulo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsAttiva = table.Column<bool>(type: "boolean", nullable: false),
                    OggettoEmailDefault = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_ConfigurazioniNotificaEmail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DestinatariNotificaEmail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigurazioneNotificaEmailId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    RepartoId = table.Column<Guid>(type: "uuid", nullable: true),
                    Ruolo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UtenteId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    Ordine = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_DestinatariNotificaEmail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DestinatariNotificaEmail_AspNetUsers_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DestinatariNotificaEmail_ConfigurazioniNotificaEmail_Config~",
                        column: x => x.ConfigurazioneNotificaEmailId,
                        principalTable: "ConfigurazioniNotificaEmail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DestinatariNotificaEmail_Reparti_RepartoId",
                        column: x => x.RepartoId,
                        principalTable: "Reparti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurazioniNotificaEmail_Codice",
                table: "ConfigurazioniNotificaEmail",
                column: "Codice",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurazioniNotificaEmail_CreatedAt",
                table: "ConfigurazioniNotificaEmail",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurazioniNotificaEmail_IsAttiva",
                table: "ConfigurazioniNotificaEmail",
                column: "IsAttiva");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurazioniNotificaEmail_IsDeleted",
                table: "ConfigurazioniNotificaEmail",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurazioniNotificaEmail_Modulo",
                table: "ConfigurazioniNotificaEmail",
                column: "Modulo");

            migrationBuilder.CreateIndex(
                name: "IX_DestinatariNotificaEmail_ConfigurazioneNotificaEmailId",
                table: "DestinatariNotificaEmail",
                column: "ConfigurazioneNotificaEmailId");

            migrationBuilder.CreateIndex(
                name: "IX_DestinatariNotificaEmail_CreatedAt",
                table: "DestinatariNotificaEmail",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DestinatariNotificaEmail_IsDeleted",
                table: "DestinatariNotificaEmail",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_DestinatariNotificaEmail_RepartoId",
                table: "DestinatariNotificaEmail",
                column: "RepartoId");

            migrationBuilder.CreateIndex(
                name: "IX_DestinatariNotificaEmail_Tipo",
                table: "DestinatariNotificaEmail",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_DestinatariNotificaEmail_UtenteId",
                table: "DestinatariNotificaEmail",
                column: "UtenteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DestinatariNotificaEmail");

            migrationBuilder.DropTable(
                name: "ConfigurazioniNotificaEmail");
        }
    }
}
