using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità Notifica
    /// Gestisce il sistema di notifiche in-app ed email con destinatario singolo
    /// </summary>
    public class NotificaConfig : BaseEntityConfig<Notifica>
    {
        public override void Configure(EntityTypeBuilder<Notifica> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Nome tabella
            builder.ToTable("Notifiche");

            // Chiave primaria
            builder.HasKey(n => n.Id);

            // ===== DESTINATARIO =====

            builder.Property(n => n.DestinatarioUserId)
                   .IsRequired()
                   .HasMaxLength(450); // Lunghezza standard per UserId di Identity

            // ===== TIPO E CONTENUTO =====

            builder.Property(n => n.Tipo)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(n => n.Titolo)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(n => n.Messaggio)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(n => n.Link)
                   .HasMaxLength(500);

            // ===== STATO LETTURA =====

            builder.Property(n => n.IsLetta)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(n => n.DataLettura)
                   .HasColumnType("timestamp without time zone");

            // ===== STATO EMAIL =====

            builder.Property(n => n.IsInviataEmail)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(n => n.DataInvioEmail)
                   .HasColumnType("timestamp without time zone");

            // ===== RIFERIMENTI OPZIONALI =====

            builder.Property(n => n.GaraId);

            builder.Property(n => n.LottoId);

            builder.Property(n => n.ScadenzaId);

            // ===== RELAZIONI =====

            // Relazione con ApplicationUser per destinatario (Many-to-One obbligatoria)
            builder.HasOne(n => n.Destinatario)
                   .WithMany()
                   .HasForeignKey(n => n.DestinatarioUserId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella notifiche con utente

            // Relazione con Gara (Many-to-One opzionale)
            builder.HasOne(n => n.Gara)
                   .WithMany()
                   .HasForeignKey(n => n.GaraId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella notifiche con gara

            // Relazione con Lotto (Many-to-One opzionale)
            builder.HasOne(n => n.Lotto)
                   .WithMany()
                   .HasForeignKey(n => n.LottoId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella notifiche con lotto

            // Relazione con Scadenza (Many-to-One opzionale)
            builder.HasOne(n => n.Scadenza)
                   .WithMany()
                   .HasForeignKey(n => n.ScadenzaId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella notifiche con scadenza

            // ===== INDICI =====

            // Indice su DestinatarioUserId (CRITICO per query "le mie notifiche")
            builder.HasIndex(n => n.DestinatarioUserId)
                   .HasDatabaseName("IX_Notifiche_DestinatarioUserId");

            // Indice su IsLetta per filtrare notifiche non lette
            builder.HasIndex(n => n.IsLetta)
                   .HasDatabaseName("IX_Notifiche_IsLetta");

            // Indice su Tipo per filtri per tipo di notifica
            builder.HasIndex(n => n.Tipo)
                   .HasDatabaseName("IX_Notifiche_Tipo");

            // Indice su IsInviataEmail per job di invio email
            builder.HasIndex(n => n.IsInviataEmail)
                   .HasDatabaseName("IX_Notifiche_IsInviataEmail");

            // Indice su DataLettura per analisi temporali
            builder.HasIndex(n => n.DataLettura)
                   .HasDatabaseName("IX_Notifiche_DataLettura");

            // Indice su CreatedAt (da BaseEntity) per ordinamento temporale
            builder.HasIndex(n => n.CreatedAt)
                   .HasDatabaseName("IX_Notifiche_CreatedAt");

            // Indice composto per query "notifiche non lette per utente" (CRITICO)
            builder.HasIndex(n => new { n.DestinatarioUserId, n.IsLetta })
                   .HasDatabaseName("IX_Notifiche_DestinatarioUserId_IsLetta");

            // Indice composto per query "notifiche per utente ordinate per data"
            builder.HasIndex(n => new { n.DestinatarioUserId, n.CreatedAt })
                   .HasDatabaseName("IX_Notifiche_DestinatarioUserId_CreatedAt");

            // Indice composto per job invio email (notifiche non inviate)
            builder.HasIndex(n => new { n.IsInviataEmail, n.CreatedAt })
                   .HasDatabaseName("IX_Notifiche_IsInviataEmail_CreatedAt");

            // Indice su GaraId per query rapide per gara
            builder.HasIndex(n => n.GaraId)
                   .HasDatabaseName("IX_Notifiche_GaraId");

            // Indice su LottoId per query rapide per lotto
            builder.HasIndex(n => n.LottoId)
                   .HasDatabaseName("IX_Notifiche_LottoId");

            // Indice su ScadenzaId per query rapide per scadenza
            builder.HasIndex(n => n.ScadenzaId)
                   .HasDatabaseName("IX_Notifiche_ScadenzaId");
        }
    }
}