using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità ElaborazioneLotto
    /// Gestisce la fase di elaborazione con prezzi desiderati e reali
    /// </summary>
    public class ElaborazioneLottoConfig : BaseEntityConfig<ElaborazioneLotto>
    {
        public override void Configure(EntityTypeBuilder<ElaborazioneLotto> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Nome tabella
            builder.ToTable("ElaborazioniLotti");

            // Chiave primaria
            builder.HasKey(e => e.Id);

            // ===== FOREIGN KEY =====

            builder.Property(e => e.LottoId)
                   .IsRequired();

            // ===== PREZZI =====

            builder.Property(e => e.PrezzoDesiderato)
                   .HasPrecision(18, 2);

            builder.Property(e => e.PrezzoRealeUscita)
                   .HasPrecision(18, 2);

            // ===== NOTE E MOTIVAZIONI =====

            builder.Property(e => e.MotivazioneAdattamento)
                   .HasMaxLength(2000);

            builder.Property(e => e.Note)
                   .HasMaxLength(2000);

            // ===== RELAZIONI =====

            // Relazione con Lotto (One-to-One obbligatoria)
            // La configurazione della relazione è già definita in LottoConfig
            // Qui definiamo solo i dettagli dal lato ElaborazioneLotto
            builder.HasOne(e => e.Lotto)
                   .WithOne(l => l.Elaborazione)
                   .HasForeignKey<ElaborazioneLotto>(e => e.LottoId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella elaborazione con lotto

            // ===== INDICI =====

            // Indice univoco su LottoId per garantire relazione One-to-One
            builder.HasIndex(e => e.LottoId)
                   .IsUnique()
                   .HasDatabaseName("IX_ElaborazioniLotti_LottoId")
                   .HasFilter("\"IsDeleted\" = false");  // CRITICO per soft delete

            // Indice su PrezzoDesiderato per analisi e report
            builder.HasIndex(e => e.PrezzoDesiderato)
                   .HasDatabaseName("IX_ElaborazioniLotti_PrezzoDesiderato");

            // Indice su PrezzoRealeUscita per analisi e report
            builder.HasIndex(e => e.PrezzoRealeUscita)
                   .HasDatabaseName("IX_ElaborazioniLotti_PrezzoRealeUscita");
        }
    }
}