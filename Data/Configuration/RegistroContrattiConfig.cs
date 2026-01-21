using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità RegistroContratti
    /// </summary>
    public class RegistroContrattiConfig : BaseEntityConfig<RegistroContratti>
    {
        public override void Configure(EntityTypeBuilder<RegistroContratti> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Nome tabella
            builder.ToTable("RegistroContratti");

            // Chiave primaria
            builder.HasKey(r => r.Id);

            // ===================================
            // PROPRIETÀ - IDENTIFICAZIONE
            // ===================================

            builder.Property(r => r.IdRiferimentoEsterno)
                .HasMaxLength(50);

            builder.Property(r => r.NumeroProtocollo)
                .HasMaxLength(50);

            builder.Property(r => r.TipoRegistro)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            // ===================================
            // PROPRIETÀ - CLIENTE
            // ===================================

            builder.Property(r => r.ClienteId);

            builder.Property(r => r.RagioneSociale)
                .HasMaxLength(200);

            builder.Property(r => r.TipoControparte)
                .HasConversion<string>()
                .HasMaxLength(20);

            // ===================================
            // PROPRIETÀ - CONTENUTO
            // ===================================

            builder.Property(r => r.Oggetto)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(r => r.CategoriaContrattoId)
                .IsRequired();

            // ===================================
            // PROPRIETÀ - RESPONSABILE INTERNO
            // ===================================

            builder.Property(r => r.UtenteId)
                .HasMaxLength(450);

            builder.Property(r => r.ResponsabileInterno)
                .HasMaxLength(100);

            // ===================================
            // PROPRIETÀ - DATE
            // ===================================

            builder.Property(r => r.DataDocumento)
                .IsRequired()
                .HasColumnType("timestamp without time zone");

            builder.Property(r => r.DataDecorrenza)
                .HasColumnType("timestamp without time zone");

            builder.Property(r => r.DataFineOScadenza)
                .HasColumnType("timestamp without time zone");

            builder.Property(r => r.DataInvio)
                .HasColumnType("timestamp without time zone");

            builder.Property(r => r.DataAccettazione)
                .HasColumnType("timestamp without time zone");

            // ===================================
            // PROPRIETÀ - SCADENZE E RINNOVI
            // ===================================

            builder.Property(r => r.GiorniPreavvisoDisdetta);

            builder.Property(r => r.GiorniAlertScadenza)
                .IsRequired()
                .HasDefaultValue(60);

            builder.Property(r => r.IsRinnovoAutomatico)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(r => r.GiorniRinnovoAutomatico);

            // ===================================
            // PROPRIETÀ - IMPORTI
            // ===================================

            builder.Property(r => r.ImportoCanoneAnnuo)
                .HasPrecision(18, 2);

            builder.Property(r => r.ImportoUnatantum)
                .HasPrecision(18, 2);

            // ===================================
            // PROPRIETÀ - STATO
            // ===================================

            builder.Property(r => r.Stato)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            // ===================================
            // PROPRIETÀ - GERARCHIA
            // ===================================

            builder.Property(r => r.ParentId);

            // ===================================
            // COMPUTED PROPERTIES - IGNORE
            // ===================================

            builder.Ignore(r => r.DataLimiteDisdetta);
            builder.Ignore(r => r.DataAlertScadenza);
            builder.Ignore(r => r.ImportoTotale);

            // ===================================
            // RELAZIONI
            // ===================================

            // Relazione con Soggetto (Cliente)
            builder.HasOne(r => r.Cliente)
                .WithMany()
                .HasForeignKey(r => r.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relazione con CategoriaContratto
            builder.HasOne(r => r.CategoriaContratto)
                .WithMany(c => c.RegistriContratti)
                .HasForeignKey(r => r.CategoriaContrattoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relazione con ApplicationUser (Responsabile)
            builder.HasOne(r => r.Utente)
                .WithMany()
                .HasForeignKey(r => r.UtenteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relazione self-referencing (Parent-Children)
            builder.HasOne(r => r.Parent)
                .WithMany(r => r.Children)
                .HasForeignKey(r => r.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===================================
            // INDICI
            // ===================================

            // Indice su IdRiferimentoEsterno per ricerca
            builder.HasIndex(r => r.IdRiferimentoEsterno);

            // Indice su NumeroProtocollo (univoco se non cancellato)
            builder.HasIndex(r => r.NumeroProtocollo)
                .IsUnique()
                .HasFilter("\"NumeroProtocollo\" IS NOT NULL AND \"IsDeleted\" = false");

            // Indice composito per ricerche frequenti
            builder.HasIndex(r => new { r.ClienteId, r.Stato });

            // Indice per scadenze (usato dal job)
            builder.HasIndex(r => r.DataFineOScadenza);

            // Indice per tipo registro
            builder.HasIndex(r => r.TipoRegistro);

            // Indice per navigazione gerarchica
            builder.HasIndex(r => r.ParentId);

            // Indice per categoria
            builder.HasIndex(r => r.CategoriaContrattoId);
        }
    }
}