using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità AllegatoRegistro
    /// </summary>
    public class AllegatoRegistroConfig : BaseEntityConfig<AllegatoRegistro>
    {
        public override void Configure(EntityTypeBuilder<AllegatoRegistro> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Nome tabella
            builder.ToTable("AllegatiRegistro");

            // Chiave primaria
            builder.HasKey(a => a.Id);

            // ===================================
            // PROPRIETÀ - RIFERIMENTO REGISTRO
            // ===================================

            builder.Property(a => a.RegistroContrattiId)
                .IsRequired();

            // ===================================
            // PROPRIETÀ - TIPO DOCUMENTO
            // ===================================

            builder.Property(a => a.TipoDocumentoId)
                .IsRequired();

            builder.Property(a => a.Descrizione)
                .HasMaxLength(500);

            // ===================================
            // PROPRIETÀ - INFO FILE
            // ===================================

            builder.Property(a => a.NomeFile)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.PathMinIO)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(a => a.DimensioneBytes)
                .IsRequired();

            builder.Property(a => a.MimeType)
                .IsRequired()
                .HasMaxLength(100);

            // ===================================
            // PROPRIETÀ - STATO UPLOAD
            // ===================================

            builder.Property(a => a.IsUploadCompleto)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(a => a.DataCaricamento)
                .IsRequired()
                .HasColumnType("timestamp without time zone");

            builder.Property(a => a.CaricatoDaUserId)
                .IsRequired()
                .HasMaxLength(450);

            // ===================================
            // RELAZIONI
            // ===================================

            // Relazione con RegistroContratti
            builder.HasOne(a => a.RegistroContratti)
                .WithMany(r => r.Allegati)
                .HasForeignKey(a => a.RegistroContrattiId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relazione con TipoDocumento
            builder.HasOne(a => a.TipoDocumento)
                .WithMany()
                .HasForeignKey(a => a.TipoDocumentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relazione con ApplicationUser (CaricatoDa)
            builder.HasOne(a => a.CaricatoDa)
                .WithMany()
                .HasForeignKey(a => a.CaricatoDaUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===================================
            // INDICI
            // ===================================

            // Indice per ricerca allegati di un registro
            builder.HasIndex(a => a.RegistroContrattiId);

            // Indice per tipo documento
            builder.HasIndex(a => a.TipoDocumentoId);

            // Indice per upload incompleti (utile per cleanup)
            builder.HasIndex(a => a.IsUploadCompleto);

            // Indice per data caricamento
            builder.HasIndex(a => a.DataCaricamento);
        }
    }
}