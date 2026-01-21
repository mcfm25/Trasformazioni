using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità AssegnazioneMezzo
    /// Supporto assegnazioni multiple in coda (rimosso vincolo UNIQUE)
    /// </summary>
    public class AssegnazioneMezzoConfig : BaseEntityConfig<AssegnazioneMezzo>
    {
        public override void Configure(EntityTypeBuilder<AssegnazioneMezzo> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Nome tabella
            builder.ToTable("AssegnazioniMezzi");

            // Chiave primaria
            builder.HasKey(a => a.Id);

            // Proprietà obbligatorie
            builder.Property(a => a.MezzoId)
                   .IsRequired();

            builder.Property(a => a.UtenteId)
                   .IsRequired()
                   .HasMaxLength(450); // Lunghezza standard per UserId di Identity

            builder.Property(a => a.DataInizio)
                   .HasColumnType("timestamp without time zone")
                   .IsRequired();

            builder.Property(a => a.MotivoAssegnazione)
                   .IsRequired();

            // Proprietà opzionali
            builder.Property(a => a.DataFine)
                   .HasColumnType("timestamp without time zone");

            builder.Property(a => a.ChilometraggioInizio)
                   .HasPrecision(10, 2);

            builder.Property(a => a.ChilometraggioFine)
                   .HasPrecision(10, 2);

            builder.Property(a => a.Note)
                   .HasMaxLength(1000);

            // Relazioni (Foreign Keys)

            // Relazione con Mezzo (Many-to-One)
            builder.HasOne(a => a.Mezzo)
                   .WithMany(m => m.Assegnazioni)
                   .HasForeignKey(a => a.MezzoId)
                   .OnDelete(DeleteBehavior.Restrict); // Non cancellare mezzo se ha assegnazioni

            // Relazione con ApplicationUser (Many-to-One)
            builder.HasOne(a => a.Utente)
                   .WithMany(u => u.AssegnazioniMezzi)
                   .HasForeignKey(a => a.UtenteId)
                   .OnDelete(DeleteBehavior.Restrict); // Non cancellare utente se ha assegnazioni

            // Indici per performance

            // Indice su MezzoId per query "assegnazioni per mezzo"
            builder.HasIndex(a => a.MezzoId);

            // Indice su UtenteId per query "assegnazioni per utente"
            builder.HasIndex(a => a.UtenteId);

            // MODIFICATO: Indice composito per trovare assegnazioni attive
            // RIMOSSO .IsUnique() per permettere assegnazioni multiple temporanee
            // RIMOSSO filtro con NOW() che causava errore (funzione non deterministica)
            builder.HasIndex(a => new { a.MezzoId, a.DataFine });

            // Indice per validazione sovrapposizioni (performance ottimizzata)
            builder.HasIndex(a => new { a.MezzoId, a.DataInizio, a.DataFine })
                   .HasDatabaseName("IX_AssegnazioniMezzi_Mezzo_Periodi");

            // Indice su DataInizio per query temporali
            builder.HasIndex(a => a.DataInizio);

            // Indice su DataFine per query temporali
            builder.HasIndex(a => a.DataFine);

            // Indice composito per storico utente
            builder.HasIndex(a => new { a.UtenteId, a.DataInizio });
        }
    }
}