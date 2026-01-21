using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità Scadenza
    /// Gestisce lo scadenzario con supporto per scadenze automatiche e manuali
    /// CRITICO: monitorate da background job giornaliero
    /// </summary>
    public class ScadenzaConfig : BaseEntityConfig<Scadenza>
    {
        public override void Configure(EntityTypeBuilder<Scadenza> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Nome tabella
            builder.ToTable("Scadenze");

            // Chiave primaria
            builder.HasKey(s => s.Id);

            // ===== FOREIGN KEYS OPZIONALI =====
            // Una scadenza può essere associata a una sola di queste entità

            builder.Property(s => s.GaraId);

            builder.Property(s => s.LottoId);

            builder.Property(s => s.PreventivoId);

            // ===== TIPO E DESCRIZIONE =====

            builder.Property(s => s.Tipo)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(s => s.Descrizione)
                   .IsRequired()
                   .HasMaxLength(500);

            // ===== DATE =====

            builder.Property(s => s.DataScadenza)
                   .HasColumnType("timestamp without time zone")
                   .IsRequired();

            builder.Property(s => s.DataCompletamento)
                   .HasColumnType("timestamp without time zone");

            // ===== FLAGS =====

            builder.Property(s => s.IsAutomatica)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(s => s.IsCompletata)
                   .IsRequired()
                   .HasDefaultValue(false);

            // ===== PREAVVISO =====

            builder.Property(s => s.GiorniPreavviso)
                   .IsRequired()
                   .HasDefaultValue(0);

            // ===== NOTE =====

            builder.Property(s => s.Note)
                   .HasMaxLength(2000);

            // ===== RELAZIONI =====

            // Relazione con Gara (Many-to-One opzionale)
            builder.HasOne(s => s.Gara)
                   .WithMany()
                   .HasForeignKey(s => s.GaraId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella scadenze con gara

            // Relazione con Lotto (Many-to-One opzionale)
            builder.HasOne(s => s.Lotto)
                   .WithMany()
                   .HasForeignKey(s => s.LottoId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella scadenze con lotto

            // Relazione con Preventivo (Many-to-One opzionale)
            builder.HasOne(s => s.Preventivo)
                   .WithMany()
                   .HasForeignKey(s => s.PreventivoId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella scadenze con preventivo

            // ===== INDICI =====

            // Indice su DataScadenza (CRITICO per background job)
            // Il job giornaliero cerca scadenze imminenti
            builder.HasIndex(s => s.DataScadenza)
                   .HasDatabaseName("IX_Scadenze_DataScadenza");

            // Indice su IsCompletata per filtrare scadenze attive
            builder.HasIndex(s => s.IsCompletata)
                   .HasDatabaseName("IX_Scadenze_IsCompletata");

            // Indice su Tipo per filtri per tipo di scadenza
            builder.HasIndex(s => s.Tipo)
                   .HasDatabaseName("IX_Scadenze_Tipo");

            // Indice su IsAutomatica per distinguere scadenze automatiche/manuali
            builder.HasIndex(s => s.IsAutomatica)
                   .HasDatabaseName("IX_Scadenze_IsAutomatica");

            // Indice su GaraId per query rapide per gara
            builder.HasIndex(s => s.GaraId)
                   .HasDatabaseName("IX_Scadenze_GaraId");

            // Indice su LottoId per query rapide per lotto
            builder.HasIndex(s => s.LottoId)
                   .HasDatabaseName("IX_Scadenze_LottoId");

            // Indice su PreventivoId per query rapide per preventivo
            builder.HasIndex(s => s.PreventivoId)
                   .HasDatabaseName("IX_Scadenze_PreventivoId");

            // Indice composto per monitoraggio scadenze attive (CRITICO per background job)
            builder.HasIndex(s => new { s.IsCompletata, s.DataScadenza })
                   .HasDatabaseName("IX_Scadenze_IsCompletata_DataScadenza");

            // Indice composto per query filtrate per gara e stato
            builder.HasIndex(s => new { s.GaraId, s.IsCompletata })
                   .HasDatabaseName("IX_Scadenze_GaraId_IsCompletata");

            // Indice composto per query filtrate per lotto e stato
            builder.HasIndex(s => new { s.LottoId, s.IsCompletata })
                   .HasDatabaseName("IX_Scadenze_LottoId_IsCompletata");

            // Indice composto per scadenze imminenti con preavviso
            builder.HasIndex(s => new { s.DataScadenza, s.GiorniPreavviso, s.IsCompletata })
                   .HasDatabaseName("IX_Scadenze_DataScadenza_GiorniPreavviso_IsCompletata");
        }
    }
}