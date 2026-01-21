using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità ValutazioneLotto
    /// Gestisce la valutazione tecnica ed economica di un lotto
    /// </summary>
    public class ValutazioneLottoConfig : BaseEntityConfig<ValutazioneLotto>
    {
        public override void Configure(EntityTypeBuilder<ValutazioneLotto> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Nome tabella
            builder.ToTable("ValutazioniLotti");

            // Chiave primaria
            builder.HasKey(v => v.Id);

            // ===== FOREIGN KEY =====

            builder.Property(v => v.LottoId)
                   .IsRequired();

            // ===== VALUTAZIONE TECNICA =====

            builder.Property(v => v.DataValutazioneTecnica)
                   .HasColumnType("timestamp without time zone");

            builder.Property(v => v.ValutatoreTecnicoId)
                   .HasMaxLength(450); // Lunghezza standard per UserId di Identity

            builder.Property(v => v.TecnicaApprovata);

            builder.Property(v => v.MotivoRifiutoTecnico)
                   .HasMaxLength(2000);

            builder.Property(v => v.NoteTecniche)
                   .HasMaxLength(2000);

            // ===== VALUTAZIONE ECONOMICA =====

            builder.Property(v => v.DataValutazioneEconomica)
                   .HasColumnType("timestamp without time zone");

            builder.Property(v => v.ValutatoreEconomicoId)
                   .HasMaxLength(450); // Lunghezza standard per UserId di Identity

            builder.Property(v => v.EconomicaApprovata);

            builder.Property(v => v.MotivoRifiutoEconomico)
                   .HasMaxLength(2000);

            builder.Property(v => v.NoteEconomiche)
                   .HasMaxLength(2000);

            // ===== RELAZIONI =====

            // Relazione con Lotto (One-to-One obbligatoria)
            // La configurazione della relazione è già definita in LottoConfig
            // Qui definiamo solo i dettagli dal lato ValutazioneLotto
            builder.HasOne(v => v.Lotto)
                   .WithOne(l => l.Valutazione)
                   .HasForeignKey<ValutazioneLotto>(v => v.LottoId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella valutazione con lotto

            // Relazione con ApplicationUser per valutatore tecnico (Many-to-One opzionale)
            builder.HasOne(v => v.ValutatoreTecnico)
                   .WithMany()
                   .HasForeignKey(v => v.ValutatoreTecnicoId)
                   .OnDelete(DeleteBehavior.SetNull); // Disassocia valutatore se cancellato

            // Relazione con ApplicationUser per valutatore economico (Many-to-One opzionale)
            builder.HasOne(v => v.ValutatoreEconomico)
                   .WithMany()
                   .HasForeignKey(v => v.ValutatoreEconomicoId)
                   .OnDelete(DeleteBehavior.SetNull); // Disassocia valutatore se cancellato

            // ===== INDICI =====

            // Indice univoco su LottoId per garantire relazione One-to-One
            builder.HasIndex(v => v.LottoId)
                   .IsUnique()
                   .HasDatabaseName("IX_ValutazioniLotti_LottoId");

            // Indice sul valutatore tecnico per query di assegnazione
            builder.HasIndex(v => v.ValutatoreTecnicoId)
                   .HasDatabaseName("IX_ValutazioniLotti_ValutatoreTecnicoId");

            // Indice sul valutatore economico per query di assegnazione
            builder.HasIndex(v => v.ValutatoreEconomicoId)
                   .HasDatabaseName("IX_ValutazioniLotti_ValutatoreEconomicoId");

            // Indice sulla data valutazione tecnica per monitoraggio temporale
            builder.HasIndex(v => v.DataValutazioneTecnica)
                   .HasDatabaseName("IX_ValutazioniLotti_DataValutazioneTecnica");

            // Indice sulla data valutazione economica per monitoraggio temporale
            builder.HasIndex(v => v.DataValutazioneEconomica)
                   .HasDatabaseName("IX_ValutazioniLotti_DataValutazioneEconomica");

            // Indice su approvazione tecnica per report e statistiche
            builder.HasIndex(v => v.TecnicaApprovata)
                   .HasDatabaseName("IX_ValutazioniLotti_TecnicaApprovata");

            // Indice su approvazione economica per report e statistiche
            builder.HasIndex(v => v.EconomicaApprovata)
                   .HasDatabaseName("IX_ValutazioniLotti_EconomicaApprovata");
        }
    }
}