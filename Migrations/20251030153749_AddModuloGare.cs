using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trasformazioni.Migrations
{
    /// <inheritdoc />
    public partial class AddModuloGare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gare",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodiceGara = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Titolo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Tipologia = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Stato = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EnteAppaltante = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Regione = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NomePuntoOrdinante = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TelefonoPuntoOrdinante = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CIG = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CUP = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    RDO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Bando = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DenominazioneIniziativa = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Procedura = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CriterioAggiudicazione = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DataPubblicazione = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataInizioPresentazioneOfferte = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataTermineRichiestaChiarimenti = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataTerminePresentazioneOfferte = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ImportoTotaleStimato = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    LinkPiattaforma = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsChiusaManualmente = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DataChiusuraManuale = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MotivoChiusuraManuale = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ChiusaDaUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
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
                    table.PrimaryKey("PK_Gare", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gare_AspNetUsers_ChiusaDaUserId",
                        column: x => x.ChiusaDaUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Lotti",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GaraId = table.Column<Guid>(type: "uuid", nullable: false),
                    CodiceLotto = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Tipologia = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Stato = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MotivoRifiuto = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    LinkPiattaforma = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OperatoreAssegnatoId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    GiorniFornitura = table.Column<int>(type: "integer", nullable: true),
                    ImportoBaseAsta = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Quotazione = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    DurataContratto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DataStipulaContratto = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataScadenzaContratto = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Fatturazione = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RichiedeFideiussione = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DataInizioEsameEnte = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                    table.PrimaryKey("PK_Lotti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lotti_AspNetUsers_OperatoreAssegnatoId",
                        column: x => x.OperatoreAssegnatoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Lotti_Gare_GaraId",
                        column: x => x.GaraId,
                        principalTable: "Gare",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ElaborazioniLotti",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LottoId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrezzoDesiderato = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    PrezzoRealeUscita = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    MotivazioneAdattamento = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_ElaborazioniLotti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElaborazioniLotti_Lotti_LottoId",
                        column: x => x.LottoId,
                        principalTable: "Lotti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartecipantiLotti",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LottoId = table.Column<Guid>(type: "uuid", nullable: false),
                    SoggettoId = table.Column<Guid>(type: "uuid", nullable: true),
                    RagioneSociale = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OffertaEconomica = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    IsAggiudicatario = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsScartatoDallEnte = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_PartecipantiLotti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartecipantiLotti_Lotti_LottoId",
                        column: x => x.LottoId,
                        principalTable: "Lotti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartecipantiLotti_Soggetti_SoggettoId",
                        column: x => x.SoggettoId,
                        principalTable: "Soggetti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Preventivi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LottoId = table.Column<Guid>(type: "uuid", nullable: false),
                    SoggettoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ImportoOfferto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    DataRichiesta = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataRicezione = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataScadenza = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GiorniAutoRinnovo = table.Column<int>(type: "integer", nullable: true),
                    Stato = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DocumentPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    NomeFile = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsSelezionato = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_Preventivi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Preventivi_Lotti_LottoId",
                        column: x => x.LottoId,
                        principalTable: "Lotti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Preventivi_Soggetti_SoggettoId",
                        column: x => x.SoggettoId,
                        principalTable: "Soggetti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RichiesteIntegrazione",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LottoId = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroProgressivo = table.Column<int>(type: "integer", nullable: false),
                    DataRichiestaEnte = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TestoRichiestaEnte = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    DocumentoRichiestaPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NomeFileRichiesta = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DataRispostaAzienda = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TestoRispostaAzienda = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    DocumentoRispostaPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NomeFileRisposta = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RispostaDaUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    IsChiusa = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
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
                    table.PrimaryKey("PK_RichiesteIntegrazione", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RichiesteIntegrazione_AspNetUsers_RispostaDaUserId",
                        column: x => x.RispostaDaUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RichiesteIntegrazione_Lotti_LottoId",
                        column: x => x.LottoId,
                        principalTable: "Lotti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ValutazioniLotti",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LottoId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataValutazioneTecnica = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ValutatoreTecnicoId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    TecnicaApprovata = table.Column<bool>(type: "boolean", nullable: true),
                    MotivoRifiutoTecnico = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    NoteTecniche = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DataValutazioneEconomica = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ValutatoreEconomicoId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    EconomicaApprovata = table.Column<bool>(type: "boolean", nullable: true),
                    MotivoRifiutoEconomico = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    NoteEconomiche = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_ValutazioniLotti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValutazioniLotti_AspNetUsers_ValutatoreEconomicoId",
                        column: x => x.ValutatoreEconomicoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ValutazioniLotti_AspNetUsers_ValutatoreTecnicoId",
                        column: x => x.ValutatoreTecnicoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ValutazioniLotti_Lotti_LottoId",
                        column: x => x.LottoId,
                        principalTable: "Lotti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scadenze",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GaraId = table.Column<Guid>(type: "uuid", nullable: true),
                    LottoId = table.Column<Guid>(type: "uuid", nullable: true),
                    PreventivoId = table.Column<Guid>(type: "uuid", nullable: true),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DataScadenza = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsAutomatica = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsCompletata = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DataCompletamento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GiorniPreavviso = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_Scadenze", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scadenze_Gare_GaraId",
                        column: x => x.GaraId,
                        principalTable: "Gare",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scadenze_Lotti_LottoId",
                        column: x => x.LottoId,
                        principalTable: "Lotti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scadenze_Preventivi_PreventivoId",
                        column: x => x.PreventivoId,
                        principalTable: "Preventivi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentiGara",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GaraId = table.Column<Guid>(type: "uuid", nullable: true),
                    LottoId = table.Column<Guid>(type: "uuid", nullable: true),
                    PreventivoId = table.Column<Guid>(type: "uuid", nullable: true),
                    IntegrazioneId = table.Column<Guid>(type: "uuid", nullable: true),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NomeFile = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PathMinIO = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DimensioneBytes = table.Column<long>(type: "bigint", nullable: false),
                    MimeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descrizione = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_DocumentiGara", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentiGara_AspNetUsers_CaricatoDaUserId",
                        column: x => x.CaricatoDaUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentiGara_Gare_GaraId",
                        column: x => x.GaraId,
                        principalTable: "Gare",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentiGara_Lotti_LottoId",
                        column: x => x.LottoId,
                        principalTable: "Lotti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentiGara_Preventivi_PreventivoId",
                        column: x => x.PreventivoId,
                        principalTable: "Preventivi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentiGara_RichiesteIntegrazione_IntegrazioneId",
                        column: x => x.IntegrazioneId,
                        principalTable: "RichiesteIntegrazione",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifiche",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DestinatarioUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Titolo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Messaggio = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Link = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsLetta = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DataLettura = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsInviataEmail = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DataInvioEmail = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GaraId = table.Column<Guid>(type: "uuid", nullable: true),
                    LottoId = table.Column<Guid>(type: "uuid", nullable: true),
                    ScadenzaId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_Notifiche", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifiche_AspNetUsers_DestinatarioUserId",
                        column: x => x.DestinatarioUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifiche_Gare_GaraId",
                        column: x => x.GaraId,
                        principalTable: "Gare",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifiche_Lotti_LottoId",
                        column: x => x.LottoId,
                        principalTable: "Lotti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifiche_Scadenze_ScadenzaId",
                        column: x => x.ScadenzaId,
                        principalTable: "Scadenze",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_CaricatoDaUserId",
                table: "DocumentiGara",
                column: "CaricatoDaUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_CreatedAt",
                table: "DocumentiGara",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_DataCaricamento",
                table: "DocumentiGara",
                column: "DataCaricamento");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_GaraId",
                table: "DocumentiGara",
                column: "GaraId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_GaraId_Tipo",
                table: "DocumentiGara",
                columns: new[] { "GaraId", "Tipo" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_IntegrazioneId",
                table: "DocumentiGara",
                column: "IntegrazioneId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_IsDeleted",
                table: "DocumentiGara",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_LottoId",
                table: "DocumentiGara",
                column: "LottoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_LottoId_Tipo",
                table: "DocumentiGara",
                columns: new[] { "LottoId", "Tipo" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_PathMinIO",
                table: "DocumentiGara",
                column: "PathMinIO");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_PreventivoId",
                table: "DocumentiGara",
                column: "PreventivoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentiGara_Tipo",
                table: "DocumentiGara",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_ElaborazioniLotti_CreatedAt",
                table: "ElaborazioniLotti",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ElaborazioniLotti_IsDeleted",
                table: "ElaborazioniLotti",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ElaborazioniLotti_LottoId",
                table: "ElaborazioniLotti",
                column: "LottoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElaborazioniLotti_PrezzoDesiderato",
                table: "ElaborazioniLotti",
                column: "PrezzoDesiderato");

            migrationBuilder.CreateIndex(
                name: "IX_ElaborazioniLotti_PrezzoRealeUscita",
                table: "ElaborazioniLotti",
                column: "PrezzoRealeUscita");

            migrationBuilder.CreateIndex(
                name: "IX_Gare_ChiusaDaUserId",
                table: "Gare",
                column: "ChiusaDaUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Gare_CIG",
                table: "Gare",
                column: "CIG");

            migrationBuilder.CreateIndex(
                name: "IX_Gare_CodiceGara",
                table: "Gare",
                column: "CodiceGara",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gare_CreatedAt",
                table: "Gare",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Gare_DataPubblicazione",
                table: "Gare",
                column: "DataPubblicazione");

            migrationBuilder.CreateIndex(
                name: "IX_Gare_IsDeleted",
                table: "Gare",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Gare_Stato",
                table: "Gare",
                column: "Stato");

            migrationBuilder.CreateIndex(
                name: "IX_Gare_Stato_DataPubblicazione",
                table: "Gare",
                columns: new[] { "Stato", "DataPubblicazione" });

            migrationBuilder.CreateIndex(
                name: "IX_Lotti_CreatedAt",
                table: "Lotti",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Lotti_DataInizioEsameEnte",
                table: "Lotti",
                column: "DataInizioEsameEnte");

            migrationBuilder.CreateIndex(
                name: "IX_Lotti_GaraId_CodiceLotto",
                table: "Lotti",
                columns: new[] { "GaraId", "CodiceLotto" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lotti_GaraId_Stato",
                table: "Lotti",
                columns: new[] { "GaraId", "Stato" });

            migrationBuilder.CreateIndex(
                name: "IX_Lotti_IsDeleted",
                table: "Lotti",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Lotti_OperatoreAssegnatoId",
                table: "Lotti",
                column: "OperatoreAssegnatoId");

            migrationBuilder.CreateIndex(
                name: "IX_Lotti_Stato",
                table: "Lotti",
                column: "Stato");

            migrationBuilder.CreateIndex(
                name: "IX_Notifiche_CreatedAt",
                table: "Notifiche",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifiche_DataLettura",
                table: "Notifiche",
                column: "DataLettura");

            migrationBuilder.CreateIndex(
                name: "IX_Notifiche_DestinatarioUserId",
                table: "Notifiche",
                column: "DestinatarioUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifiche_DestinatarioUserId_CreatedAt",
                table: "Notifiche",
                columns: new[] { "DestinatarioUserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifiche_DestinatarioUserId_IsLetta",
                table: "Notifiche",
                columns: new[] { "DestinatarioUserId", "IsLetta" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifiche_GaraId",
                table: "Notifiche",
                column: "GaraId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifiche_IsDeleted",
                table: "Notifiche",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Notifiche_IsInviataEmail",
                table: "Notifiche",
                column: "IsInviataEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Notifiche_IsInviataEmail_CreatedAt",
                table: "Notifiche",
                columns: new[] { "IsInviataEmail", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifiche_IsLetta",
                table: "Notifiche",
                column: "IsLetta");

            migrationBuilder.CreateIndex(
                name: "IX_Notifiche_LottoId",
                table: "Notifiche",
                column: "LottoId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifiche_ScadenzaId",
                table: "Notifiche",
                column: "ScadenzaId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifiche_Tipo",
                table: "Notifiche",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_PartecipantiLotti_CreatedAt",
                table: "PartecipantiLotti",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PartecipantiLotti_IsAggiudicatario",
                table: "PartecipantiLotti",
                column: "IsAggiudicatario");

            migrationBuilder.CreateIndex(
                name: "IX_PartecipantiLotti_IsDeleted",
                table: "PartecipantiLotti",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PartecipantiLotti_IsScartatoDallEnte",
                table: "PartecipantiLotti",
                column: "IsScartatoDallEnte");

            migrationBuilder.CreateIndex(
                name: "IX_PartecipantiLotti_LottoId",
                table: "PartecipantiLotti",
                column: "LottoId");

            migrationBuilder.CreateIndex(
                name: "IX_PartecipantiLotti_LottoId_IsAggiudicatario",
                table: "PartecipantiLotti",
                columns: new[] { "LottoId", "IsAggiudicatario" });

            migrationBuilder.CreateIndex(
                name: "IX_PartecipantiLotti_LottoId_IsScartatoDallEnte",
                table: "PartecipantiLotti",
                columns: new[] { "LottoId", "IsScartatoDallEnte" });

            migrationBuilder.CreateIndex(
                name: "IX_PartecipantiLotti_SoggettoId",
                table: "PartecipantiLotti",
                column: "SoggettoId");

            migrationBuilder.CreateIndex(
                name: "IX_Preventivi_CreatedAt",
                table: "Preventivi",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Preventivi_DataScadenza",
                table: "Preventivi",
                column: "DataScadenza");

            migrationBuilder.CreateIndex(
                name: "IX_Preventivi_DataScadenza_Stato",
                table: "Preventivi",
                columns: new[] { "DataScadenza", "Stato" });

            migrationBuilder.CreateIndex(
                name: "IX_Preventivi_IsDeleted",
                table: "Preventivi",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Preventivi_IsSelezionato",
                table: "Preventivi",
                column: "IsSelezionato");

            migrationBuilder.CreateIndex(
                name: "IX_Preventivi_LottoId",
                table: "Preventivi",
                column: "LottoId");

            migrationBuilder.CreateIndex(
                name: "IX_Preventivi_LottoId_Stato",
                table: "Preventivi",
                columns: new[] { "LottoId", "Stato" });

            migrationBuilder.CreateIndex(
                name: "IX_Preventivi_SoggettoId",
                table: "Preventivi",
                column: "SoggettoId");

            migrationBuilder.CreateIndex(
                name: "IX_Preventivi_Stato",
                table: "Preventivi",
                column: "Stato");

            migrationBuilder.CreateIndex(
                name: "IX_RichiesteIntegrazione_CreatedAt",
                table: "RichiesteIntegrazione",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RichiesteIntegrazione_DataRichiestaEnte",
                table: "RichiesteIntegrazione",
                column: "DataRichiestaEnte");

            migrationBuilder.CreateIndex(
                name: "IX_RichiesteIntegrazione_IsChiusa",
                table: "RichiesteIntegrazione",
                column: "IsChiusa");

            migrationBuilder.CreateIndex(
                name: "IX_RichiesteIntegrazione_IsChiusa_DataRichiestaEnte",
                table: "RichiesteIntegrazione",
                columns: new[] { "IsChiusa", "DataRichiestaEnte" });

            migrationBuilder.CreateIndex(
                name: "IX_RichiesteIntegrazione_IsDeleted",
                table: "RichiesteIntegrazione",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_RichiesteIntegrazione_LottoId",
                table: "RichiesteIntegrazione",
                column: "LottoId");

            migrationBuilder.CreateIndex(
                name: "IX_RichiesteIntegrazione_LottoId_IsChiusa",
                table: "RichiesteIntegrazione",
                columns: new[] { "LottoId", "IsChiusa" });

            migrationBuilder.CreateIndex(
                name: "IX_RichiesteIntegrazione_LottoId_NumeroProgressivo",
                table: "RichiesteIntegrazione",
                columns: new[] { "LottoId", "NumeroProgressivo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RichiesteIntegrazione_RispostaDaUserId",
                table: "RichiesteIntegrazione",
                column: "RispostaDaUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Scadenze_CreatedAt",
                table: "Scadenze",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Scadenze_DataScadenza",
                table: "Scadenze",
                column: "DataScadenza");

            migrationBuilder.CreateIndex(
                name: "IX_Scadenze_DataScadenza_GiorniPreavviso_IsCompletata",
                table: "Scadenze",
                columns: new[] { "DataScadenza", "GiorniPreavviso", "IsCompletata" });

            migrationBuilder.CreateIndex(
                name: "IX_Scadenze_GaraId",
                table: "Scadenze",
                column: "GaraId");

            migrationBuilder.CreateIndex(
                name: "IX_Scadenze_GaraId_IsCompletata",
                table: "Scadenze",
                columns: new[] { "GaraId", "IsCompletata" });

            migrationBuilder.CreateIndex(
                name: "IX_Scadenze_IsAutomatica",
                table: "Scadenze",
                column: "IsAutomatica");

            migrationBuilder.CreateIndex(
                name: "IX_Scadenze_IsCompletata",
                table: "Scadenze",
                column: "IsCompletata");

            migrationBuilder.CreateIndex(
                name: "IX_Scadenze_IsCompletata_DataScadenza",
                table: "Scadenze",
                columns: new[] { "IsCompletata", "DataScadenza" });

            migrationBuilder.CreateIndex(
                name: "IX_Scadenze_IsDeleted",
                table: "Scadenze",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Scadenze_LottoId",
                table: "Scadenze",
                column: "LottoId");

            migrationBuilder.CreateIndex(
                name: "IX_Scadenze_LottoId_IsCompletata",
                table: "Scadenze",
                columns: new[] { "LottoId", "IsCompletata" });

            migrationBuilder.CreateIndex(
                name: "IX_Scadenze_PreventivoId",
                table: "Scadenze",
                column: "PreventivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Scadenze_Tipo",
                table: "Scadenze",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_ValutazioniLotti_CreatedAt",
                table: "ValutazioniLotti",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ValutazioniLotti_DataValutazioneEconomica",
                table: "ValutazioniLotti",
                column: "DataValutazioneEconomica");

            migrationBuilder.CreateIndex(
                name: "IX_ValutazioniLotti_DataValutazioneTecnica",
                table: "ValutazioniLotti",
                column: "DataValutazioneTecnica");

            migrationBuilder.CreateIndex(
                name: "IX_ValutazioniLotti_EconomicaApprovata",
                table: "ValutazioniLotti",
                column: "EconomicaApprovata");

            migrationBuilder.CreateIndex(
                name: "IX_ValutazioniLotti_IsDeleted",
                table: "ValutazioniLotti",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ValutazioniLotti_LottoId",
                table: "ValutazioniLotti",
                column: "LottoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ValutazioniLotti_TecnicaApprovata",
                table: "ValutazioniLotti",
                column: "TecnicaApprovata");

            migrationBuilder.CreateIndex(
                name: "IX_ValutazioniLotti_ValutatoreEconomicoId",
                table: "ValutazioniLotti",
                column: "ValutatoreEconomicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ValutazioniLotti_ValutatoreTecnicoId",
                table: "ValutazioniLotti",
                column: "ValutatoreTecnicoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentiGara");

            migrationBuilder.DropTable(
                name: "ElaborazioniLotti");

            migrationBuilder.DropTable(
                name: "Notifiche");

            migrationBuilder.DropTable(
                name: "PartecipantiLotti");

            migrationBuilder.DropTable(
                name: "ValutazioniLotti");

            migrationBuilder.DropTable(
                name: "RichiesteIntegrazione");

            migrationBuilder.DropTable(
                name: "Scadenze");

            migrationBuilder.DropTable(
                name: "Preventivi");

            migrationBuilder.DropTable(
                name: "Lotti");

            migrationBuilder.DropTable(
                name: "Gare");
        }
    }
}
