using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità per la gestione documenti tramite MinIO
    /// Documenti possono essere associati a Gara, Lotto, Preventivo o Integrazione
    /// Path in MinIO segue pattern: gare/{garaId}/lotti/{lottoId}/{guid}_{filename}
    /// </summary>
    public class DocumentoGara : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid? GaraId { get; set; }
        public Guid? LottoId { get; set; }
        public Guid? PreventivoId { get; set; }
        public Guid? IntegrazioneId { get; set; }

        // rimosso
        //public TipoDocumentoGara Tipo { get; set; }

        // Passaggio tramite FK a TipoDocumento per maggiore flessibilità
        public Guid TipoDocumentoId { get; set; }
        public virtual TipoDocumento TipoDocumento { get; set; } = null!;

        public string NomeFile { get; set; } = string.Empty;
        public string PathMinIO { get; set; } = string.Empty;
        public long DimensioneBytes { get; set; }
        public string MimeType { get; set; } = string.Empty;
        public string? Descrizione { get; set; }

        public DateTime DataCaricamento { get; set; }
        public string CaricatoDaUserId { get; set; } = string.Empty;

        // ===== RELAZIONI =====
        public Gara? Gara { get; set; }
        public Lotto? Lotto { get; set; }
        public Preventivo? Preventivo { get; set; }
        public RichiestaIntegrazione? Integrazione { get; set; }
        public ApplicationUser CaricatoDa { get; set; } = null!;

        /// <summary>
        /// Indica se l'upload del file su MinIO è stato completato con successo.
        /// Usato per gestire transazioni distribuite DB/MinIO.
        /// </summary>
        public bool IsUploadCompleto { get; set; } = false;
    }
}