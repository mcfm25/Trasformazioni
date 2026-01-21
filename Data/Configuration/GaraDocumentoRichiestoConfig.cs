using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità GaraDocumentoRichiesto.
    /// Gestisce la checklist dei documenti richiesti per ogni gara.
    /// </summary>
    public class GaraDocumentoRichiestoConfig : IEntityTypeConfiguration<GaraDocumentoRichiesto>
    {
        public void Configure(EntityTypeBuilder<GaraDocumentoRichiesto> builder)
        {
            // ===== TABELLA =====
            builder.ToTable("GaraDocumentiRichiesti");

            // ===== CHIAVE PRIMARIA =====
            builder.HasKey(x => x.Id);

            // ===== PROPRIETÀ =====
            builder.Property(x => x.GaraId)
                   .IsRequired();

            builder.Property(x => x.TipoDocumentoId)
                   .IsRequired();

            // ===== RELAZIONI =====

            // Relazione con Gara (Many-to-One)
            builder.HasOne(x => x.Gara)
                   .WithMany(g => g.DocumentiRichiesti)
                   .HasForeignKey(x => x.GaraId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relazione con TipoDocumento (Many-to-One)
            builder.HasOne(x => x.TipoDocumento)
                   .WithMany()
                   .HasForeignKey(x => x.TipoDocumentoId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ===== INDICI =====

            // Indice univoco su GaraId + TipoDocumentoId (no duplicati per gara)
            builder.HasIndex(x => new { x.GaraId, x.TipoDocumentoId })
                   .IsUnique()
                   .HasDatabaseName("IX_GaraDocumentiRichiesti_GaraId_TipoDocumentoId");

            // Indice su GaraId per query rapide
            builder.HasIndex(x => x.GaraId)
                   .HasDatabaseName("IX_GaraDocumentiRichiesti_GaraId");

            // Indice su TipoDocumentoId
            builder.HasIndex(x => x.TipoDocumentoId)
                   .HasDatabaseName("IX_GaraDocumentiRichiesti_TipoDocumentoId");
        }
    }
}