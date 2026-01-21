using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità Lotto
    /// Ogni lotto rappresenta una suddivisione della gara con workflow indipendente
    /// </summary>
    public class LottoConfig : BaseEntityConfig<Lotto>
    {
        public override void Configure(EntityTypeBuilder<Lotto> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Nome tabella
            builder.ToTable("Lotti");

            // Chiave primaria
            builder.HasKey(l => l.Id);

            // ===== IDENTIFICAZIONE =====

            builder.Property(l => l.GaraId)
                   .IsRequired();

            builder.Property(l => l.CodiceLotto)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(l => l.Descrizione)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(l => l.Tipologia)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(l => l.Stato)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            // ===== RIFIUTO =====

            builder.Property(l => l.MotivoRifiuto)
                   .HasMaxLength(1000);

            // ===== INFO GENERALI =====

            builder.Property(l => l.LinkPiattaforma)
                   .HasMaxLength(500);

            builder.Property(l => l.OperatoreAssegnatoId)
                   .HasMaxLength(450); // Lunghezza standard per UserId di Identity

            builder.Property(l => l.GiorniFornitura);

            // ===== INFO ECONOMICHE =====

            builder.Property(l => l.ImportoBaseAsta)
                   .HasPrecision(18, 2);

            builder.Property(l => l.Quotazione)
                   .HasPrecision(18, 2);

            // ===== INFO CONTRATTO =====

            builder.Property(l => l.DurataContratto)
                   .HasMaxLength(200);

            builder.Property(l => l.DataStipulaContratto)
                   .HasColumnType("timestamp without time zone");

            builder.Property(l => l.DataScadenzaContratto)
                   .HasColumnType("timestamp without time zone");

            builder.Property(l => l.Fatturazione)
                   .HasMaxLength(200);

            // ===== INFO PARTECIPAZIONE =====

            builder.Property(l => l.RichiedeFideiussione)
                   .IsRequired()
                   .HasDefaultValue(false);

            // ===== ESAME ENTE =====

            builder.Property(l => l.DataInizioEsameEnte)
                   .HasColumnType("timestamp without time zone");

            // ===== RELAZIONI =====

            // Relazione con Gara (Many-to-One obbligatoria)
            builder.HasOne(l => l.Gara)
                   .WithMany(g => g.Lotti)
                   .HasForeignKey(l => l.GaraId)
                   .OnDelete(DeleteBehavior.Restrict); // Non cancellare gara se ha lotti

            // Relazione con ApplicationUser per operatore assegnato (Many-to-One opzionale)
            builder.HasOne(l => l.OperatoreAssegnato)
                   .WithMany()
                   .HasForeignKey(l => l.OperatoreAssegnatoId)
                   .OnDelete(DeleteBehavior.SetNull); // Disassocia operatore se cancellato

            // Relazione con ValutazioneLotto (One-to-One opzionale)
            builder.HasOne(l => l.Valutazione)
                   .WithOne(v => v.Lotto)
                   .HasForeignKey<ValutazioneLotto>(v => v.LottoId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella valutazione con lotto

            // Relazione con ElaborazioneLotto (One-to-One opzionale)
            builder.HasOne(l => l.Elaborazione)
                   .WithOne(e => e.Lotto)
                   .HasForeignKey<ElaborazioneLotto>(e => e.LottoId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella elaborazione con lotto

            // Relazione con Preventivi (One-to-Many)
            builder.HasMany(l => l.Preventivi)
                   .WithOne(p => p.Lotto)
                   .HasForeignKey(p => p.LottoId)
                   .OnDelete(DeleteBehavior.Restrict); // Non cancellare lotto se ha preventivi

            // Relazione con RichiesteIntegrazione (One-to-Many)
            builder.HasMany(l => l.RichiesteIntegrazione)
                   .WithOne(r => r.Lotto)
                   .HasForeignKey(r => r.LottoId)
                   .OnDelete(DeleteBehavior.Restrict); // Non cancellare lotto se ha richieste

            // Relazione con PartecipanteLotto (One-to-Many)
            builder.HasMany(l => l.Partecipanti)
                   .WithOne(p => p.Lotto)
                   .HasForeignKey(p => p.LottoId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella partecipanti con lotto

            // Relazione con DocumentoGara (One-to-Many)
            builder.HasMany(l => l.Documenti)
                   .WithOne(d => d.Lotto)
                   .HasForeignKey(d => d.LottoId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella documenti con lotto

            // ===== INDICI =====

            // Indice univoco composto su GaraId e CodiceLotto
            // Un lotto deve avere codice univoco all'interno della sua gara
            builder.HasIndex(l => new { l.GaraId, l.CodiceLotto })
                   .IsUnique()
                   .HasDatabaseName("IX_Lotti_GaraId_CodiceLotto");

            // Indice sullo stato per filtri rapidi
            builder.HasIndex(l => l.Stato)
                   .HasDatabaseName("IX_Lotti_Stato");

            // Indice sull'operatore assegnato per query di assegnazione
            builder.HasIndex(l => l.OperatoreAssegnatoId)
                   .HasDatabaseName("IX_Lotti_OperatoreAssegnatoId");

            // Indice sulla data inizio esame ente per monitoraggio stati
            builder.HasIndex(l => l.DataInizioEsameEnte)
                   .HasDatabaseName("IX_Lotti_DataInizioEsameEnte");

            // Indice composto per query filtrate per gara e stato
            builder.HasIndex(l => new { l.GaraId, l.Stato })
                   .HasDatabaseName("IX_Lotti_GaraId_Stato");
        }
    }
}