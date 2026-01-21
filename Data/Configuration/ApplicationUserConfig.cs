using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Configuration
{
    public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            //builder.Property(e => e.)
            //       .IsRequired()
            //       .HasMaxLength(100);
            //builder.Property(e => e.CreatedAt)
            //       .HasDefaultValueSql("CURRENT_TIMESTAMP");
            // Proprietà richieste
            builder.Property(e => e.CreatedAt)
                   .HasColumnType("timestamp without time zone")
                   .IsRequired();

            builder.Property(e => e.ModifiedAt)
                   .HasColumnType("timestamp without time zone");

            builder.Property(e => e.DeletedAt)
                   .HasColumnType("timestamp without time zone");

            //builder.Property(e => e.LockoutEnd)
            //       .HasColumnType("timestamp without time zone");

            builder.Property(e => e.DataAssunzione)
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
