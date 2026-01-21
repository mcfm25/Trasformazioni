using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità Gara
    /// Gestisce le gare d'appalto con tutti i dati amministrativi e le relazioni
    /// </summary>
    public class GaraConfig : BaseEntityConfig<Gara>
    {
        public override void Configure(EntityTypeBuilder<Gara> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Nome tabella
            builder.ToTable("Gare");

            // Chiave primaria
            builder.HasKey(g => g.Id);

            // ===== IDENTIFICAZIONE =====

            builder.Property(g => g.CodiceGara)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(g => g.Titolo)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(g => g.Descrizione)
                   .HasMaxLength(2000);

            builder.Property(g => g.Tipologia)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(g => g.Stato)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            // ===== INFO AMMINISTRAZIONE =====

            builder.Property(g => g.EnteAppaltante)
                   .HasMaxLength(200);

            builder.Property(g => g.Regione)
                   .HasMaxLength(100);

            builder.Property(g => g.NomePuntoOrdinante)
                   .HasMaxLength(200);

            builder.Property(g => g.TelefonoPuntoOrdinante)
                   .HasMaxLength(50);

            // ===== CODICI GARA =====

            builder.Property(g => g.CIG)
                   .HasMaxLength(20);

            builder.Property(g => g.CUP)
                   .HasMaxLength(20);

            builder.Property(g => g.RDO)
                   .HasMaxLength(50);

            builder.Property(g => g.Bando)
                   .HasMaxLength(100);

            builder.Property(g => g.DenominazioneIniziativa)
                   .HasMaxLength(500);

            builder.Property(g => g.Procedura)
                   .HasMaxLength(200);

            builder.Property(g => g.CriterioAggiudicazione)
                   .HasMaxLength(200);

            // ===== DATE CRITICHE =====

            builder.Property(g => g.DataPubblicazione)
                   .HasColumnType("timestamp without time zone");

            builder.Property(g => g.DataInizioPresentazioneOfferte)
                   .HasColumnType("timestamp without time zone");

            builder.Property(g => g.DataTermineRichiestaChiarimenti)
                   .HasColumnType("timestamp without time zone");

            builder.Property(g => g.DataTerminePresentazioneOfferte)
                   .HasColumnType("timestamp without time zone");

            // ===== INFO ECONOMICHE =====

            builder.Property(g => g.ImportoTotaleStimato)
                   .HasPrecision(18, 2);

            // ===== LINK =====

            builder.Property(g => g.LinkPiattaforma)
                   .HasMaxLength(500);

            // ===== CHIUSURA MANUALE =====

            builder.Property(g => g.IsChiusaManualmente)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(g => g.DataChiusuraManuale)
                   .HasColumnType("timestamp without time zone");

            builder.Property(g => g.MotivoChiusuraManuale)
                   .HasMaxLength(1000);

            builder.Property(g => g.ChiusaDaUserId)
                   .HasMaxLength(450); // Lunghezza standard per UserId di Identity

            // ===== RELAZIONI =====

            // Relazione con ApplicationUser per chiusura manuale (Many-to-One opzionale)
            builder.HasOne(g => g.ChiusaDa)
                   .WithMany()
                   .HasForeignKey(g => g.ChiusaDaUserId)
                   .OnDelete(DeleteBehavior.Restrict); // Non cancellare utente se ha chiuso gare

            // Relazione con Lotti (One-to-Many)
            builder.HasMany(g => g.Lotti)
                   .WithOne(l => l.Gara)
                   .HasForeignKey(l => l.GaraId)
                   .OnDelete(DeleteBehavior.Restrict); // Non cancellare gara se ha lotti associati

            // Relazione con DocumentoGara (One-to-Many)
            builder.HasMany(g => g.Documenti)
                   .WithOne(d => d.Gara)
                   .HasForeignKey(d => d.GaraId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella documenti quando si cancella la gara

            // ===== INDICI =====

            // Indice univoco sul codice gara
            builder.HasIndex(g => g.CodiceGara)
                   .IsUnique()
                   .HasDatabaseName("IX_Gare_CodiceGara");

            // Indice sullo stato per filtri rapidi
            builder.HasIndex(g => g.Stato)
                   .HasDatabaseName("IX_Gare_Stato");

            // Indice sulla data di pubblicazione per query temporali
            builder.HasIndex(g => g.DataPubblicazione)
                   .HasDatabaseName("IX_Gare_DataPubblicazione");

            // Indice composto per query filtrate per stato e data
            builder.HasIndex(g => new { g.Stato, g.DataPubblicazione })
                   .HasDatabaseName("IX_Gare_Stato_DataPubblicazione");

            // Indice sul CIG per ricerche rapide (nullable ma comunque utile)
            builder.HasIndex(g => g.CIG)
                   .HasDatabaseName("IX_Gare_CIG");
        }
    }
}