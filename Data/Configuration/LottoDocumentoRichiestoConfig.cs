using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Configuration
{
    /// <summary>
    /// Configurazione Entity Framework per l'entità LottoDocumentoRichiesto.
    /// Gestisce la checklist dei documenti richiesti per ogni lotto.
    /// </summary>
    public class LottoDocumentoRichiestoConfig : IEntityTypeConfiguration<LottoDocumentoRichiesto>
    {
        public void Configure(EntityTypeBuilder<LottoDocumentoRichiesto> builder)
        {
            builder.ToTable("LottoDocumentiRichiesti");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.LottoId).IsRequired();
            builder.Property(x => x.TipoDocumentoId).IsRequired();

            // Relazioni
            builder.HasOne(x => x.Lotto)
                   .WithMany(l => l.DocumentiRichiesti)
                   .HasForeignKey(x => x.LottoId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.TipoDocumento)
                   .WithMany()
                   .HasForeignKey(x => x.TipoDocumentoId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indice univoco
            builder.HasIndex(x => new { x.LottoId, x.TipoDocumentoId })
                   .IsUnique()
                   .HasDatabaseName("IX_LottoDocumentiRichiesti_LottoId_TipoDocumentoId");

            builder.HasIndex(x => x.LottoId)
                   .HasDatabaseName("IX_LottoDocumentiRichiesti_LottoId");

            builder.HasIndex(x => x.TipoDocumentoId)
                   .HasDatabaseName("IX_LottoDocumentiRichiesti_TipoDocumentoId");
        }
    }
}
