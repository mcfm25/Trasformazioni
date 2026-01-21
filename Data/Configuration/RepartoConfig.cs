using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Data.Configuration;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Configurations
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità Reparto
    /// </summary>
    public class RepartoConfig : BaseEntityConfig<Reparto>
    {
        public override void Configure(EntityTypeBuilder<Reparto> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Tabella
            builder.ToTable("Reparti");

            // Chiave primaria
            builder.HasKey(r => r.Id);

            // Proprietà
            builder.Property(r => r.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(r => r.Descrizione)
                .HasMaxLength(500);

            // Indici
            builder.HasIndex(r => r.Nome)
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");

            builder.HasIndex(r => r.Email)
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");

            builder.HasIndex(r => r.IsDeleted);

            // Query filter per soft delete
            builder.HasQueryFilter(r => !r.IsDeleted);

            // Relazione con ApplicationUser
            builder.HasMany(r => r.Utenti)
                .WithOne(u => u.Reparto)
                .HasForeignKey(u => u.RepartoId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}