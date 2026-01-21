using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione base per tutte le entità che ereditano da BaseEntity.
    /// Applica configurazioni comuni come filtri globali per soft delete e convenzioni per i campi audit.
    /// </summary>
    /// <typeparam name="TEntity">Tipo dell'entità che eredita da BaseEntity</typeparam>
    public abstract class BaseEntityConfig<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            // Proprietà richieste
            builder.Property(e => e.CreatedAt)
                   .HasColumnType("timestamp without time zone")
                   .IsRequired();

            builder.Property(e => e.ModifiedAt)
                   .HasColumnType("timestamp without time zone");

            builder.Property(e => e.DeletedAt)
                   .HasColumnType("timestamp without time zone");

            builder.Property(e => e.CreatedBy)
                   .IsRequired()
                   .HasMaxLength(450); // Lunghezza standard per UserId di Identity

            builder.Property(e => e.ModifiedBy)
                   .HasMaxLength(450);

            builder.Property(e => e.DeletedBy)
                   .HasMaxLength(450);

            builder.Property(e => e.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);

            // Filtro globale per soft delete
            // Esclude automaticamente le entità con IsDeleted = true da tutte le query
            builder.HasQueryFilter(e => !e.IsDeleted);

            // Indici per migliorare le performance
            builder.HasIndex(e => e.IsDeleted);
            builder.HasIndex(e => e.CreatedAt);
        }
    }
}