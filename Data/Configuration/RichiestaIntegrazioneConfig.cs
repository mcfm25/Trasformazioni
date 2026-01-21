using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità RichiestaIntegrazione
    /// Gestisce il ping-pong di richieste/risposte integrazioni con l'ente
    /// </summary>
    public class RichiestaIntegrazioneConfig : BaseEntityConfig<RichiestaIntegrazione>
    {
        public override void Configure(EntityTypeBuilder<RichiestaIntegrazione> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Nome tabella
            builder.ToTable("RichiesteIntegrazione");

            // Chiave primaria
            builder.HasKey(r => r.Id);

            // ===== FOREIGN KEY =====

            builder.Property(r => r.LottoId)
                   .IsRequired();

            // ===== NUMERO PROGRESSIVO =====

            builder.Property(r => r.NumeroProgressivo)
                   .IsRequired();

            // ===== RICHIESTA ENTE =====

            builder.Property(r => r.DataRichiestaEnte)
                   .HasColumnType("timestamp without time zone")
                   .IsRequired();

            builder.Property(r => r.TestoRichiestaEnte)
                   .IsRequired()
                   .HasMaxLength(4000);

            builder.Property(r => r.DocumentoRichiestaPath)
                   .HasMaxLength(500);

            builder.Property(r => r.NomeFileRichiesta)
                   .HasMaxLength(255);

            // ===== RISPOSTA AZIENDA =====

            builder.Property(r => r.DataRispostaAzienda)
                   .HasColumnType("timestamp without time zone");

            builder.Property(r => r.TestoRispostaAzienda)
                   .HasMaxLength(4000);

            builder.Property(r => r.DocumentoRispostaPath)
                   .HasMaxLength(500);

            builder.Property(r => r.NomeFileRisposta)
                   .HasMaxLength(255);

            builder.Property(r => r.RispostaDaUserId)
                   .HasMaxLength(450); // Lunghezza standard per UserId di Identity

            // ===== STATO =====

            builder.Property(r => r.IsChiusa)
                   .IsRequired()
                   .HasDefaultValue(false);

            // ===== RELAZIONI =====

            // Relazione con Lotto (Many-to-One obbligatoria)
            builder.HasOne(r => r.Lotto)
                   .WithMany(l => l.RichiesteIntegrazione)
                   .HasForeignKey(r => r.LottoId)
                   .OnDelete(DeleteBehavior.Restrict); // Non cancellare lotto se ha richieste

            // Relazione con ApplicationUser per risposta (Many-to-One opzionale)
            builder.HasOne(r => r.RispostaDa)
                   .WithMany()
                   .HasForeignKey(r => r.RispostaDaUserId)
                   .OnDelete(DeleteBehavior.SetNull); // Disassocia utente se cancellato

            // ===== INDICI =====

            // Indice univoco composto su LottoId e NumeroProgressivo
            // Ogni richiesta deve avere numero univoco all'interno del lotto
            builder.HasIndex(r => new { r.LottoId, r.NumeroProgressivo })
                   .IsUnique()
                   .HasDatabaseName("IX_RichiesteIntegrazione_LottoId_NumeroProgressivo");

            // Indice su LottoId per query rapide per lotto
            builder.HasIndex(r => r.LottoId)
                   .HasDatabaseName("IX_RichiesteIntegrazione_LottoId");

            // Indice su DataRichiestaEnte per ordinamento temporale
            builder.HasIndex(r => r.DataRichiestaEnte)
                   .HasDatabaseName("IX_RichiesteIntegrazione_DataRichiestaEnte");

            // Indice su IsChiusa per filtri rapidi
            builder.HasIndex(r => r.IsChiusa)
                   .HasDatabaseName("IX_RichiesteIntegrazione_IsChiusa");

            // Indice su RispostaDaUserId per query sulle risposte per utente
            builder.HasIndex(r => r.RispostaDaUserId)
                   .HasDatabaseName("IX_RichiesteIntegrazione_RispostaDaUserId");

            // Indice composto per query filtrate per lotto e stato
            builder.HasIndex(r => new { r.LottoId, r.IsChiusa })
                   .HasDatabaseName("IX_RichiesteIntegrazione_LottoId_IsChiusa");

            // Indice composto per monitoraggio richieste aperte con data
            builder.HasIndex(r => new { r.IsChiusa, r.DataRichiestaEnte })
                   .HasDatabaseName("IX_RichiesteIntegrazione_IsChiusa_DataRichiestaEnte");
        }
    }
}