using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità DocumentoGara
    /// Gestisce i documenti tramite MinIO con associazioni multiple (Gara, Lotto, Preventivo, Integrazione)
    /// </summary>
    public class DocumentoGaraConfig : BaseEntityConfig<DocumentoGara>
    {
        public override void Configure(EntityTypeBuilder<DocumentoGara> builder)
        {
            // Applica configurazione base (audit fields, soft delete)
            base.Configure(builder);

            // Nome tabella
            builder.ToTable("DocumentiGara");

            // Chiave primaria
            builder.HasKey(d => d.Id);

            // ===== FOREIGN KEYS OPZIONALI =====
            // Un documento può essere associato a una sola di queste entità

            builder.Property(d => d.GaraId);

            builder.Property(d => d.LottoId);

            builder.Property(d => d.PreventivoId);

            builder.Property(d => d.IntegrazioneId);

            // ===== TIPO DOCUMENTO =====
            // rimosso il campo Tipo in favore della FK TipoDocumentoId
            //builder.Property(d => d.Tipo)
            //       .IsRequired()
            //       .HasConversion<string>()
            //       .HasMaxLength(50);

            // ===== INFO FILE =====

            builder.Property(d => d.NomeFile)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(d => d.PathMinIO)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(d => d.DimensioneBytes)
                   .IsRequired();

            builder.Property(d => d.MimeType)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.Descrizione)
                   .HasMaxLength(500);

            // ===== INFO CARICAMENTO =====

            builder.Property(d => d.DataCaricamento)
                   .HasColumnType("timestamp without time zone")
                   .IsRequired();

            builder.Property(d => d.CaricatoDaUserId)
                   .IsRequired()
                   .HasMaxLength(450); // Lunghezza standard per UserId di Identity

            builder.Property(d => d.IsUploadCompleto)
                   .IsRequired()
                   .HasDefaultValue(false);

            // ===== RELAZIONI =====

            // Relazione con Gara (Many-to-One opzionale)
            builder.HasOne(d => d.Gara)
                   .WithMany(g => g.Documenti)
                   .HasForeignKey(d => d.GaraId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella documenti con gara

            // Relazione con Lotto (Many-to-One opzionale)
            builder.HasOne(d => d.Lotto)
                   .WithMany(l => l.Documenti)
                   .HasForeignKey(d => d.LottoId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella documenti con lotto

            // Relazione con Preventivo (Many-to-One opzionale)
            builder.HasOne(d => d.Preventivo)
                   .WithMany()
                   .HasForeignKey(d => d.PreventivoId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella documenti con preventivo

            // Relazione con RichiestaIntegrazione (Many-to-One opzionale)
            builder.HasOne(d => d.Integrazione)
                   .WithMany()
                   .HasForeignKey(d => d.IntegrazioneId)
                   .OnDelete(DeleteBehavior.Cascade); // Cancella documenti con integrazione

            // Relazione con ApplicationUser per caricamento (Many-to-One obbligatoria)
            builder.HasOne(d => d.CaricatoDa)
                   .WithMany()
                   .HasForeignKey(d => d.CaricatoDaUserId)
                   .OnDelete(DeleteBehavior.Restrict); // Non cancellare utente se ha caricato documenti

            // Relazione con TipoDocumento (Many-to-One obbligatoria)
            builder.HasOne(d => d.TipoDocumento)
                   .WithMany(t => t.DocumentiGara)
                   .HasForeignKey(d => d.TipoDocumentoId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ===== INDICI =====

            // Indice su GaraId per query rapide per gara
            builder.HasIndex(d => d.GaraId)
                   .HasDatabaseName("IX_DocumentiGara_GaraId");

            // Indice su LottoId per query rapide per lotto
            builder.HasIndex(d => d.LottoId)
                   .HasDatabaseName("IX_DocumentiGara_LottoId");

            // Indice su PreventivoId per query rapide per preventivo
            builder.HasIndex(d => d.PreventivoId)
                   .HasDatabaseName("IX_DocumentiGara_PreventivoId");

            // Indice su IntegrazioneId per query rapide per integrazione
            builder.HasIndex(d => d.IntegrazioneId)
                   .HasDatabaseName("IX_DocumentiGara_IntegrazioneId");

            //// Indice su Tipo per filtri per tipo di documento
            //builder.HasIndex(d => d.Tipo)
            //       .HasDatabaseName("IX_DocumentiGara_Tipo");
            // Indice su TipoDocumentoId per filtri per tipo di documento
            builder.HasIndex(d => d.TipoDocumentoId)
                   .HasDatabaseName("IX_DocumentiGara_TipoDocumentoId");

            // Indice su DataCaricamento per ordinamento temporale
            builder.HasIndex(d => d.DataCaricamento)
                   .HasDatabaseName("IX_DocumentiGara_DataCaricamento");

            // Indice su CaricatoDaUserId per query sui documenti per utente
            builder.HasIndex(d => d.CaricatoDaUserId)
                   .HasDatabaseName("IX_DocumentiGara_CaricatoDaUserId");

            // Indice su PathMinIO per ricerca rapida del file
            builder.HasIndex(d => d.PathMinIO)
                   .HasDatabaseName("IX_DocumentiGara_PathMinIO");

            //// Indice composto per query filtrate per gara e tipo
            //builder.HasIndex(d => new { d.GaraId, d.Tipo })
            //       .HasDatabaseName("IX_DocumentiGara_GaraId_Tipo");
            // Indice composto per query filtrate per gara e tipo
            builder.HasIndex(d => new { d.GaraId, d.TipoDocumentoId })
                   .HasDatabaseName("IX_DocumentiGara_GaraId_TipoDocumentoId");

            //// Indice composto per query filtrate per lotto e tipo
            //builder.HasIndex(d => new { d.LottoId, d.Tipo })
            //       .HasDatabaseName("IX_DocumentiGara_LottoId_Tipo");
            // Indice composto per query filtrate per lotto e tipo
            builder.HasIndex(d => new { d.LottoId, d.TipoDocumentoId })
                   .HasDatabaseName("IX_DocumentiGara_LottoId_TipoDocumentoId");

            // Indice per trovare upload incompleti (per job di pulizia)
            builder.HasIndex(d => d.IsUploadCompleto)
                   .HasDatabaseName("IX_DocumentiGara_IsUploadCompleto")
                   .HasFilter("\"IsUploadCompleto\" = false");
        }
    }
}