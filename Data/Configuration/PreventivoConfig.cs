using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità Preventivo
    /// Gestisce i preventivi dei fornitori con monitoraggio scadenze e auto-rinnovo
    /// </summary>
    public class PreventivoConfig : BaseEntityConfig<Preventivo>
    {
        public override void Configure(EntityTypeBuilder<Preventivo> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Nome tabella
            builder.ToTable("Preventivi");

            // Chiave primaria
            builder.HasKey(p => p.Id);

            // ===== FOREIGN KEYS =====

            builder.Property(p => p.LottoId)
                   .IsRequired();

            builder.Property(p => p.SoggettoId)
                   .IsRequired();

            // ===== DATI PREVENTIVO =====

            builder.Property(p => p.Descrizione)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(p => p.ImportoOfferto)
                   .HasPrecision(18, 2);

            // ===== DATE =====

            builder.Property(p => p.DataRichiesta)
                   .HasColumnType("timestamp without time zone")
                   .IsRequired();

            builder.Property(p => p.DataRicezione)
                   .HasColumnType("timestamp without time zone");

            builder.Property(p => p.DataScadenza)
                   .HasColumnType("timestamp without time zone")
                   .IsRequired();

            // ===== AUTO-RINNOVO =====

            builder.Property(p => p.GiorniAutoRinnovo);

            // ===== STATO =====

            builder.Property(p => p.Stato)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            // ===== DOCUMENTO =====

            builder.Property(p => p.DocumentPath)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(p => p.NomeFile)
                   .IsRequired()
                   .HasMaxLength(255);

            // ===== SELEZIONE =====

            builder.Property(p => p.IsSelezionato)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(p => p.Note)
                   .HasMaxLength(2000);

            // ===== RELAZIONI =====

            // Relazione con Lotto (Many-to-One obbligatoria)
            builder.HasOne(p => p.Lotto)
                   .WithMany(l => l.Preventivi)
                   .HasForeignKey(p => p.LottoId)
                   .OnDelete(DeleteBehavior.Restrict); // Non cancellare lotto se ha preventivi

            // Relazione con Soggetto (Many-to-One obbligatoria)
            builder.HasOne(p => p.Soggetto)
                   .WithMany()
                   .HasForeignKey(p => p.SoggettoId)
                   .OnDelete(DeleteBehavior.Restrict); // Non cancellare soggetto se ha preventivi

            // ===== INDICI =====

            // Indice su LottoId per query rapide per lotto
            builder.HasIndex(p => p.LottoId)
                   .HasDatabaseName("IX_Preventivi_LottoId");

            // Indice su SoggettoId per query rapide per fornitore
            builder.HasIndex(p => p.SoggettoId)
                   .HasDatabaseName("IX_Preventivi_SoggettoId");

            // Indice su DataScadenza per monitoraggio scadenze (CRITICO per background job)
            builder.HasIndex(p => p.DataScadenza)
                   .HasDatabaseName("IX_Preventivi_DataScadenza");

            // Indice su Stato per filtri rapidi
            builder.HasIndex(p => p.Stato)
                   .HasDatabaseName("IX_Preventivi_Stato");

            // Indice su IsSelezionato per query sui preventivi selezionati
            builder.HasIndex(p => p.IsSelezionato)
                   .HasDatabaseName("IX_Preventivi_IsSelezionato");

            // Indice composto per query filtrate per lotto e stato
            builder.HasIndex(p => new { p.LottoId, p.Stato })
                   .HasDatabaseName("IX_Preventivi_LottoId_Stato");

            // Indice composto per monitoraggio scadenze per stato
            builder.HasIndex(p => new { p.DataScadenza, p.Stato })
                   .HasDatabaseName("IX_Preventivi_DataScadenza_Stato");
        }
    }
}