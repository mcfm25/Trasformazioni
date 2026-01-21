using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità Mezzo
    /// </summary>
    public class MezzoConfig : BaseEntityConfig<Mezzo>
    {
        public override void Configure(EntityTypeBuilder<Mezzo> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Nome tabella
            builder.ToTable("Mezzi");

            // Chiave primaria
            builder.HasKey(m => m.Id);

            // Proprietà obbligatorie
            builder.Property(m => m.Targa)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(m => m.Marca)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(m => m.Modello)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(m => m.Tipo)
                   .IsRequired();

            builder.Property(m => m.Stato)
                   .IsRequired()
                   .HasDefaultValue(StatoMezzo.Disponibile);

            builder.Property(m => m.TipoProprieta)
                   .IsRequired()
                   .HasDefaultValue(TipoProprietaMezzo.Proprieta);

            // Proprietà opzionali
            builder.Property(m => m.Chilometraggio)
                   .HasPrecision(10, 2);

            builder.Property(e => e.DataImmatricolazione)
                   .HasColumnType("timestamp without time zone");

            builder.Property(e => e.DataAcquisto)
                   .HasColumnType("timestamp without time zone");

            builder.Property(e => e.DataInizioNoleggio)
                   .HasColumnType("timestamp without time zone");

            builder.Property(e => e.DataFineNoleggio)
                   .HasColumnType("timestamp without time zone");

            builder.Property(m => m.SocietaNoleggio)
                   .HasMaxLength(100);

            builder.Property(e => e.DataScadenzaAssicurazione)
                   .HasColumnType("timestamp without time zone");

            builder.Property(e => e.DataScadenzaRevisione)
                   .HasColumnType("timestamp without time zone");

            builder.Property(m => m.DeviceIMEI)
                   .HasMaxLength(30);

            builder.Property(m => m.DevicePhoneNumber)
                   .HasMaxLength(30);

            builder.Property(m => m.Note)
                   .HasMaxLength(1000);

            // Indici
            builder.HasIndex(m => m.Targa)
                   .IsUnique()
                   .HasFilter("\"IsDeleted\" = false"); // Unique solo per record non cancellati

            builder.HasIndex(m => m.Stato);
            builder.HasIndex(m => m.TipoProprieta);
            builder.HasIndex(m => new { m.Marca, m.Modello });

            // Indici per ricerche per scadenze
            builder.HasIndex(m => m.DataScadenzaAssicurazione);
            builder.HasIndex(m => m.DataScadenzaRevisione);
        }
    }
}