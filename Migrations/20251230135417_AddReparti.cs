using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trasformazioni.Migrations
{
    /// <inheritdoc />
    public partial class AddReparti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reparto",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "RepartoId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Reparti",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_Reparti", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RepartoId",
                table: "AspNetUsers",
                column: "RepartoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reparti_CreatedAt",
                table: "Reparti",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Reparti_Email",
                table: "Reparti",
                column: "Email",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Reparti_IsDeleted",
                table: "Reparti",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Reparti_Nome",
                table: "Reparti",
                column: "Nome",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Reparti_RepartoId",
                table: "AspNetUsers",
                column: "RepartoId",
                principalTable: "Reparti",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Reparti_RepartoId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Reparti");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RepartoId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RepartoId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Reparto",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }
    }
}
