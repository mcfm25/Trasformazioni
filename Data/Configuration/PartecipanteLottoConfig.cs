using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità PartecipanteLotto
    /// Censisce i partecipanti ad un lotto (tipicamente per lotti non vinti)
    /// </summary>
    public class PartecipanteLottoConfig : BaseEntityConfig<PartecipanteLotto>
    {
        public override void Configure(EntityTypeBuilder<PartecipanteLotto> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Nome tabella
            builder.ToTable("PartecipantiLotti");

            // Chiave primaria
            builder.HasKey(p => p.Id);

            // ===== FOREIGN KEYS =====

            builder.Property(p => p.LottoId)
                   .IsRequired();

            builder.Property(p => p.SoggettoId);
            // SoggettoId è opzionale perché potremmo censire partecipanti 
            // che non sono nella nostra anagrafica soggetti

            // ===== DATI PARTECIPANTE =====

            builder.Property(p => p.RagioneSociale)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.OffertaEconomica)
                   .HasPrecision(18, 2);

            // ===== FLAGS =====

            builder.Property(p => p.IsAggiudicatario)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(p => p.IsScartatoDallEnte)
                   .IsRequired()
                   .HasDefaultValue(false);

            // ===== NOTE =====

            builder.Property(p => p.Note)
                   .HasMaxLength(2000);

            // ===== RELAZIONI =====

            // Relazione con Lotto (Many-to-One obbligatoria)
            builder.HasOne(p => p.Lotto)
                   .WithMany(l => l.Partecipanti)
                   .HasForeignKey(p => p.LottoId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella partecipanti con lotto

            // Relazione con Soggetto (Many-to-One opzionale)
            builder.HasOne(p => p.Soggetto)
                   .WithMany()
                   .HasForeignKey(p => p.SoggettoId)
                   .OnDelete(DeleteBehavior.SetNull); // Disassocia soggetto se cancellato

            // ===== INDICI =====

            // Indice su LottoId per query rapide per lotto
            builder.HasIndex(p => p.LottoId)
                   .HasDatabaseName("IX_PartecipantiLotti_LottoId");

            // Indice su SoggettoId per query rapide per soggetto
            builder.HasIndex(p => p.SoggettoId)
                   .HasDatabaseName("IX_PartecipantiLotti_SoggettoId");

            // Indice su IsAggiudicatario per trovare rapidamente i vincitori
            builder.HasIndex(p => p.IsAggiudicatario)
                   .HasDatabaseName("IX_PartecipantiLotti_IsAggiudicatario");

            // Indice su IsScartatoDallEnte per report
            builder.HasIndex(p => p.IsScartatoDallEnte)
                   .HasDatabaseName("IX_PartecipantiLotti_IsScartatoDallEnte");

            // Indice composto per trovare aggiudicatari per lotto
            builder.HasIndex(p => new { p.LottoId, p.IsAggiudicatario })
                   .HasDatabaseName("IX_PartecipantiLotti_LottoId_IsAggiudicatario");

            // Indice composto per analisi partecipanti scartati per lotto
            builder.HasIndex(p => new { p.LottoId, p.IsScartatoDallEnte })
                   .HasDatabaseName("IX_PartecipantiLotti_LottoId_IsScartatoDallEnte");
        }
    }
}