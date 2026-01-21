using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità Soggetto
    /// </summary>
    public class SoggettoConfig : BaseEntityConfig<Soggetto>
    {
        public override void Configure(EntityTypeBuilder<Soggetto> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            builder.ToTable("Soggetti");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.CodiceInterno)
                .HasMaxLength(50);

            // ===================================
            // PROPRIETÀ - CLASSIFICAZIONE
            // ===================================

            builder.Property(s => s.TipoSoggetto)
                .IsRequired();

            builder.Property(s => s.NaturaGiuridica)
                .IsRequired();

            builder.Property(s => s.IsCliente)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(s => s.IsFornitore)
                .IsRequired()
                .HasDefaultValue(false);

            // ===================================
            // PROPRIETÀ - DATI ANAGRAFICI
            // ===================================

            builder.Property(s => s.Denominazione)
                .HasMaxLength(200);

            builder.Property(s => s.Nome)
                .HasMaxLength(100);

            builder.Property(s => s.Cognome)
                .HasMaxLength(100);

            builder.Property(s => s.CodiceFiscale)
                .HasMaxLength(16);

            builder.Property(s => s.PartitaIVA)
                .HasMaxLength(20);

            builder.Property(s => s.CodiceSDI)
                .HasMaxLength(7);

            builder.Property(s => s.CodiceIPA)
                .HasMaxLength(6);

            builder.Property(s => s.Referente)
                .HasMaxLength(200);

            // ===================================
            // PROPRIETÀ - CONTATTI
            // ===================================

            builder.Property(s => s.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Telefono)
                .HasMaxLength(20);

            builder.Property(s => s.PEC)
                .HasMaxLength(100);

            // ===================================
            // PROPRIETÀ - INDIRIZZO
            // ===================================

            builder.Property(s => s.TipoVia)
                .HasMaxLength(20);

            builder.Property(s => s.NomeVia)
                .HasMaxLength(200);

            builder.Property(s => s.NumeroCivico)
                .HasMaxLength(20);

            builder.Property(s => s.Citta)
                .HasMaxLength(100);

            builder.Property(s => s.CAP)
                .HasMaxLength(10);

            builder.Property(s => s.Provincia)
                .HasMaxLength(2);

            builder.Property(s => s.Nazione)
                .HasMaxLength(50);

            // ===================================
            // PROPRIETÀ - DATI COMMERCIALI
            // ===================================

            builder.Property(s => s.CondizioniPagamento)
                .HasMaxLength(100);

            builder.Property(s => s.IBAN)
                .HasMaxLength(34);

            builder.Property(s => s.ScontoPartner)
                .HasPrecision(5, 2); // Es: 99.99%

            // ===================================
            // PROPRIETÀ - ALTRO
            // ===================================

            builder.Property(s => s.Note)
                .HasMaxLength(1000);

            // ===================================
            // COMPUTED PROPERTIES - IGNORE
            // ===================================

            builder.Ignore(s => s.NomeCompleto);
            builder.Ignore(s => s.IndirizzoCompleto);
            builder.Ignore(s => s.RuoloDescrizione);

            // ===================================
            // INDICI (SENZA FILTRI IS NOT NULL)
            // ===================================

            // Indice su CodiceInterno per ricerca rapida (unique solo se non cancellato)
            builder.HasIndex(s => s.CodiceInterno)
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");

            // Indice su PartitaIVA per ricerca rapida (senza unique)
            builder.HasIndex(s => s.PartitaIVA);

            // Indice su CodiceFiscale per ricerca rapida (senza unique)
            builder.HasIndex(s => s.CodiceFiscale);

            // Indice su Email per ricerca rapida
            builder.HasIndex(s => s.Email);

            // Indice composto per filtri comuni (tipo + ruoli)
            builder.HasIndex(s => new { s.TipoSoggetto, s.IsCliente, s.IsFornitore });
        }
    }
}