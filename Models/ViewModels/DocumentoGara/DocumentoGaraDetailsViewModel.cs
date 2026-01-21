namespace Trasformazioni.Models.ViewModels.DocumentoGara
{
    /// <summary>
    /// ViewModel per la visualizzazione dettagliata di un documento
    /// </summary>
    public class DocumentoGaraDetailsViewModel
    {
        public Guid Id { get; set; }

        // Informazioni Gara
        public Guid? GaraId { get; set; }
        public string? GaraCodice { get; set; }
        public string? GaraOggetto { get; set; }
        //public string? GaraEnteCodice { get; set; }
        public string? GaraEnteDenominazione { get; set; }

        // Informazioni Lotto
        public Guid? LottoId { get; set; }
        public string? LottoCodice { get; set; }
        public string? LottoDescrizione { get; set; }

        // Informazioni Preventivo
        public Guid? PreventivoId { get; set; }
        public string? PreventivoFornitore { get; set; }

        // Informazioni Integrazione
        public Guid? IntegrazioneId { get; set; }
        public string? IntegrazioneOggetto { get; set; }

        // Informazioni Documento
        //public TipoDocumentoGara Tipo { get; set; }
        //public string TipoDisplay { get; set; } = string.Empty;

        // DOPO
        // Informazioni Tipo Documento
        /// <summary>
        /// ID del tipo documento (da tabella TipiDocumento)
        /// </summary>
        public Guid TipoDocumentoId { get; set; }

        /// <summary>
        /// Nome del tipo documento
        /// </summary>
        public string TipoDocumentoNome { get; set; } = string.Empty;

        /// <summary>
        /// Codice riferimento per confronti nel workflow
        /// </summary>
        public string? TipoDocumentoCodiceRiferimento { get; set; }

        /// <summary>
        /// Display name formattato (manteniamo per retrocompatibilità view)
        /// </summary>
        public string TipoDisplay { get; set; } = string.Empty;

        public string NomeFile { get; set; } = string.Empty;
        public string PathMinIO { get; set; } = string.Empty;

        public long DimensioneBytes { get; set; }
        public string DimensioneFormatted { get; set; } = string.Empty;

        public string MimeType { get; set; } = string.Empty;
        public string? Descrizione { get; set; }

        // Informazioni Caricamento
        public DateTime DataCaricamento { get; set; }
        public string CaricatoDaUserId { get; set; } = string.Empty;
        public string CaricatoDaNome { get; set; } = string.Empty;
        public string CaricatoDaEmail { get; set; } = string.Empty;

        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// Icona da visualizzare in base al tipo di file
        /// </summary>
        public string FileIcon { get; set; } = "fa-file";

        /// <summary>
        /// Indica se il file può essere visualizzato in anteprima nel browser
        /// </summary>
        public bool CanPreview { get; set; }
    }
}