namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la visualizzazione del dettaglio di un allegato del registro contratti
    /// </summary>
    public class AllegatoRegistroDetailsViewModel
    {
        /// <summary>
        /// Identificatore univoco
        /// </summary>
        public Guid Id { get; set; }

        // ===================================
        // RIFERIMENTO REGISTRO
        // ===================================

        /// <summary>
        /// ID del registro contratti
        /// </summary>
        public Guid RegistroContrattiId { get; set; }

        /// <summary>
        /// Numero protocollo del registro
        /// </summary>
        public string? RegistroNumeroProtocollo { get; set; }

        /// <summary>
        /// Oggetto del registro
        /// </summary>
        public string? RegistroOggetto { get; set; }

        /// <summary>
        /// Ragione sociale del cliente del registro
        /// </summary>
        public string? RegistroRagioneSociale { get; set; }

        // ===================================
        // TIPO DOCUMENTO
        // ===================================

        /// <summary>
        /// ID del tipo documento
        /// </summary>
        public Guid TipoDocumentoId { get; set; }

        /// <summary>
        /// Nome del tipo documento
        /// </summary>
        public string TipoDocumentoNome { get; set; } = string.Empty;

        /// <summary>
        /// Descrizione dell'allegato
        /// </summary>
        public string? Descrizione { get; set; }

        // ===================================
        // INFO FILE
        // ===================================

        /// <summary>
        /// Nome originale del file
        /// </summary>
        public string NomeFile { get; set; } = string.Empty;

        /// <summary>
        /// Percorso del file in MinIO
        /// </summary>
        public string PathMinIO { get; set; } = string.Empty;

        /// <summary>
        /// Dimensione del file in bytes
        /// </summary>
        public long DimensioneBytes { get; set; }

        /// <summary>
        /// MIME type del file
        /// </summary>
        public string MimeType { get; set; } = string.Empty;

        // ===================================
        // STATO UPLOAD
        // ===================================

        /// <summary>
        /// Indica se l'upload è stato completato
        /// </summary>
        public bool IsUploadCompleto { get; set; }

        /// <summary>
        /// Data e ora del caricamento
        /// </summary>
        public DateTime DataCaricamento { get; set; }

        /// <summary>
        /// ID dell'utente che ha caricato il file
        /// </summary>
        public string CaricatoDaUserId { get; set; } = string.Empty;

        /// <summary>
        /// Nome dell'utente che ha caricato il file
        /// </summary>
        public string CaricatoDaNome { get; set; } = string.Empty;

        // ===================================
        // AUDIT
        // ===================================

        /// <summary>
        /// Data di creazione
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Utente che ha creato il record
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Data ultima modifica
        /// </summary>
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// Utente che ha modificato il record
        /// </summary>
        public string? ModifiedBy { get; set; }

        // ===================================
        // PROPRIETÀ CALCOLATE
        // ===================================

        /// <summary>
        /// Dimensione formattata (es. "1.5 MB")
        /// </summary>
        public string DimensioneFormattata
        {
            get
            {
                string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                double len = DimensioneBytes;
                int order = 0;

                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len = len / 1024;
                }

                return $"{len:0.##} {sizes[order]}";
            }
        }

        /// <summary>
        /// Icona FontAwesome basata sul MIME type
        /// </summary>
        public string FileIcon => MimeType.ToLowerInvariant() switch
        {
            "application/pdf" => "fa-file-pdf",
            "application/msword" or
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => "fa-file-word",
            "application/vnd.ms-excel" or
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => "fa-file-excel",
            "application/vnd.ms-powerpoint" or
            "application/vnd.openxmlformats-officedocument.presentationml.presentation" => "fa-file-powerpoint",
            "application/zip" or
            "application/x-zip-compressed" or
            "application/x-rar-compressed" or
            "application/x-7z-compressed" => "fa-file-zipper",
            "text/plain" => "fa-file-lines",
            var mime when mime.StartsWith("image/") => "fa-file-image",
            var mime when mime.StartsWith("video/") => "fa-file-video",
            var mime when mime.StartsWith("audio/") => "fa-file-audio",
            _ => "fa-file"
        };

        /// <summary>
        /// Colore CSS per l'icona del file
        /// </summary>
        public string FileIconColor => MimeType.ToLowerInvariant() switch
        {
            "application/pdf" => "text-danger",
            "application/msword" or
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => "text-primary",
            "application/vnd.ms-excel" or
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => "text-success",
            "application/vnd.ms-powerpoint" or
            "application/vnd.openxmlformats-officedocument.presentationml.presentation" => "text-warning",
            var mime when mime.StartsWith("image/") => "text-info",
            _ => "text-secondary"
        };

        /// <summary>
        /// Indica se il file può essere visualizzato in anteprima
        /// </summary>
        public bool CanPreview => MimeType.ToLowerInvariant() switch
        {
            "application/pdf" => true,
            var mime when mime.StartsWith("image/") => true,
            var mime when mime.StartsWith("text/") => true,
            _ => false
        };

        /// <summary>
        /// Estensione del file
        /// </summary>
        public string Estensione
        {
            get
            {
                var lastDot = NomeFile.LastIndexOf('.');
                return lastDot >= 0 ? NomeFile.Substring(lastDot).ToUpperInvariant() : "";
            }
        }

        /// <summary>
        /// Badge per lo stato upload
        /// </summary>
        public string StatoUploadBadgeClass => IsUploadCompleto
            ? "badge bg-success"
            : "badge bg-warning text-dark";

        /// <summary>
        /// Testo per lo stato upload
        /// </summary>
        public string StatoUploadDescrizione => IsUploadCompleto
            ? "Completato"
            : "In corso";

        /// <summary>
        /// Indica se l'allegato può essere eliminato
        /// </summary>
        public bool CanDelete => true;

        /// <summary>
        /// Indica se l'allegato può essere modificato (descrizione)
        /// </summary>
        public bool CanEdit => IsUploadCompleto;
    }
}