using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Data.Configurations
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità TipoDocumento.
    /// Definisce mapping, indici e vincoli per la tabella TipiDocumento.
    /// </summary>
    public class TipoDocumentoConfig : IEntityTypeConfiguration<TipoDocumento>
    {
        public void Configure(EntityTypeBuilder<TipoDocumento> builder)
        {
            // ===== TABELLA =====
            builder.ToTable("TipiDocumento");

            // ===== CHIAVE PRIMARIA =====
            builder.HasKey(t => t.Id);

            // ===== PROPRIETÀ =====

            builder.Property(t => t.Nome)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(t => t.Descrizione)
                   .HasMaxLength(500);

            // Enum salvato come stringa per leggibilità nel DB
            builder.Property(t => t.Area)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(t => t.IsSystem)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(t => t.CodiceRiferimento)
                   .HasMaxLength(50);

            // ===== AUDIT =====

            builder.Property(t => t.CreatedAt)
                   .IsRequired()
                   .HasColumnType("timestamp without time zone");

            builder.Property(t => t.CreatedBy)
                   .HasMaxLength(450);

            builder.Property(t => t.ModifiedAt)
                   .HasColumnType("timestamp without time zone");

            builder.Property(t => t.ModifiedBy)
                   .HasMaxLength(450);

            builder.Property(t => t.DeletedAt)
                   .HasColumnType("timestamp without time zone");

            builder.Property(t => t.DeletedBy)
                   .HasMaxLength(450);

            builder.Property(t => t.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);

            // ===== INDICI =====

            // Indice univoco su Nome + Area (nome univoco per area)
            builder.HasIndex(t => new { t.Nome, t.Area })
                   .IsUnique()
                   .HasDatabaseName("IX_TipiDocumento_Nome_Area")
                   .HasFilter("\"IsDeleted\" = false");  // Solo per record attivi

            // Indice su Area per filtri rapidi
            builder.HasIndex(t => t.Area)
                   .HasDatabaseName("IX_TipiDocumento_Area");

            // Indice su IsSystem per query sui tipi di sistema
            builder.HasIndex(t => t.IsSystem)
                   .HasDatabaseName("IX_TipiDocumento_IsSystem");

            // Indice su CodiceRiferimento per query nel workflow
            builder.HasIndex(t => t.CodiceRiferimento)
                   .HasDatabaseName("IX_TipiDocumento_CodiceRiferimento");

            // Indice su IsDeleted per soft delete
            builder.HasIndex(t => t.IsDeleted)
                   .HasDatabaseName("IX_TipiDocumento_IsDeleted");

            // Indice composto Area + IsDeleted per query filtrate
            builder.HasIndex(t => new { t.Area, t.IsDeleted })
                   .HasDatabaseName("IX_TipiDocumento_Area_IsDeleted");

            // Indice su CreatedAt per ordinamento cronologico
            builder.HasIndex(t => t.CreatedAt)
                   .HasDatabaseName("IX_TipiDocumento_CreatedAt");

            // ===== QUERY FILTER (Soft Delete) =====
            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}