namespace Trasformazioni.Models.ViewModels.DocumentoGara
{
    /// <summary>
    /// ViewModel per la visualizzazione in lista dei documenti
    /// </summary>
    public class DocumentoGaraListViewModel
    {
        public Guid Id { get; set; }

        public Guid? GaraId { get; set; }
        public string? GaraCodice { get; set; }
        public string? GaraOggetto { get; set; }

        public Guid? LottoId { get; set; }
        public string? LottoCodice { get; set; }
        public string? LottoDescrizione { get; set; }

        public Guid? PreventivoId { get; set; }
        public Guid? IntegrazioneId { get; set; }

        //public TipoDocumentoGara Tipo { get; set; }

        // DOPO
        /// <summary>
        /// ID del tipo documento (da tabella TipiDocumento)
        /// </summary>
        public Guid TipoDocumentoId { get; set; }
        /// <summary>
        /// Nome del tipo documento (per visualizzazione)
        /// </summary>
        public string TipoDocumentoNome { get; set; } = string.Empty;

        /// <summary>
        /// Codice riferimento del tipo documento (per confronti nel workflow)
        /// Corrisponde al nome dell'enum TipoDocumentoGara per i tipi di sistema
        /// </summary>
        public string? TipoDocumentoCodiceRiferimento { get; set; }


        /// <summary>
        /// Display name formattato (manteniamo per retrocompatibilità view)
        /// </summary>
        public string TipoDisplay { get; set; } = string.Empty;

        public string NomeFile { get; set; } = string.Empty;
        public long DimensioneBytes { get; set; }
        public string DimensioneFormatted { get; set; } = string.Empty;

        public string MimeType { get; set; } = string.Empty;
        public string? Descrizione { get; set; }

        public DateTime DataCaricamento { get; set; }
        public string CaricatoDaUserId { get; set; } = string.Empty;
        public string CaricatoDaNome { get; set; } = string.Empty;

        /// <summary>
        /// Icona da visualizzare in base al tipo di file
        /// </summary>
        public string FileIcon { get; set; } = "fa-file";
    }
}