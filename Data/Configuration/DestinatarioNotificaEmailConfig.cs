using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Data.Configuration;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Configurations
{
    /// <summary>
    /// Configurazione Entity Framework per DestinatarioNotificaEmail
    /// </summary>
    public class DestinatarioNotificaEmailConfig : BaseEntityConfig<DestinatarioNotificaEmail>
    {
        public override void Configure(EntityTypeBuilder<DestinatarioNotificaEmail> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Tabella
            builder.ToTable("DestinatariNotificaEmail");

            // Chiave primaria
            builder.HasKey(d => d.Id);

            // Proprietà
            builder.Property(d => d.Tipo)
                .IsRequired();

            builder.Property(d => d.Ruolo)
                .HasMaxLength(100);

            builder.Property(d => d.UtenteId)
                .HasMaxLength(450); // Standard Identity UserId length

            builder.Property(d => d.Note)
                .HasMaxLength(500);

            builder.Property(d => d.Ordine)
                .HasDefaultValue(0);

            // Indici
            builder.HasIndex(d => d.ConfigurazioneNotificaEmailId);

            builder.HasIndex(d => d.Tipo);

            builder.HasIndex(d => d.RepartoId);

            builder.HasIndex(d => d.UtenteId);

            builder.HasIndex(d => d.IsDeleted);

            // Query filter per soft delete
            builder.HasQueryFilter(d => !d.IsDeleted);

            // Relazione con Reparto
            builder.HasOne(d => d.Reparto)
                .WithMany()
                .HasForeignKey(d => d.RepartoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relazione con ApplicationUser
            builder.HasOne(d => d.Utente)
                .WithMany()
                .HasForeignKey(d => d.UtenteId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}