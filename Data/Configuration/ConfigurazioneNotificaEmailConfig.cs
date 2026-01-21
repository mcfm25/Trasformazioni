using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Data.Configuration;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Configurations
{
    /// <summary>
    /// Configurazione Entity Framework per ConfigurazioneNotificaEmail
    /// </summary>
    public class ConfigurazioneNotificaEmailConfig : BaseEntityConfig<ConfigurazioneNotificaEmail>
    {
        public override void Configure(EntityTypeBuilder<ConfigurazioneNotificaEmail> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Tabella
            builder.ToTable("ConfigurazioniNotificaEmail");

            // Chiave primaria
            builder.HasKey(c => c.Id);

            // Proprietà
            builder.Property(c => c.Codice)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Descrizione)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.Modulo)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.OggettoEmailDefault)
                .HasMaxLength(500);

            builder.Property(c => c.Note)
                .HasMaxLength(1000);

            // Indici
            builder.HasIndex(c => c.Codice)
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");

            builder.HasIndex(c => c.Modulo);

            builder.HasIndex(c => c.IsAttiva);

            builder.HasIndex(c => c.IsDeleted);

            // Query filter per soft delete
            builder.HasQueryFilter(c => !c.IsDeleted);

            // Relazione con Destinatari
            builder.HasMany(c => c.Destinatari)
                .WithOne(d => d.ConfigurazioneNotificaEmail)
                .HasForeignKey(d => d.ConfigurazioneNotificaEmailId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}