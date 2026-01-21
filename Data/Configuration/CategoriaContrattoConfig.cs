using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità CategoriaContratto
    /// </summary>
    public class CategoriaContrattoConfig : BaseEntityConfig<CategoriaContratto>
    {
        public override void Configure(EntityTypeBuilder<CategoriaContratto> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Nome tabella
            builder.ToTable("CategorieContratto");

            // Chiave primaria
            builder.HasKey(c => c.Id);

            // ===== PROPRIETÀ =====

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Descrizione)
                .HasMaxLength(500);

            builder.Property(c => c.Ordine)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(c => c.IsAttivo)
                .IsRequired()
                .HasDefaultValue(true);

            // ===== INDICI =====

            // Nome univoco tra le categorie attive (non cancellate)
            builder.HasIndex(c => c.Nome)
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");

            // Indice per ordinamento
            builder.HasIndex(c => c.Ordine);

            // Indice per filtro categorie attive
            builder.HasIndex(c => c.IsAttivo);
        }
    }
}