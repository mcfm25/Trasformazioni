using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per l'upload di un allegato al registro contratti
    /// </summary>
    public class AllegatoRegistroUploadViewModel
    {
        // ===================================
        // RIFERIMENTO REGISTRO
        // ===================================

        /// <summary>
        /// ID del registro contratti a cui allegare il file
        /// </summary>
        [Required(ErrorMessage = "Il registro è obbligatorio")]
        public Guid RegistroContrattiId { get; set; }

        // ===================================
        // TIPO DOCUMENTO
        // ===================================

        /// <summary>
        /// ID del tipo documento
        /// </summary>
        [Display(Name = "Tipo Documento")]
        [Required(ErrorMessage = "Il tipo documento è obbligatorio")]
        public Guid TipoDocumentoId { get; set; }

        /// <summary>
        /// Descrizione aggiuntiva dell'allegato
        /// </summary>
        [Display(Name = "Descrizione")]
        [StringLength(500, ErrorMessage = "La descrizione non può superare i 500 caratteri")]
        [DataType(DataType.MultilineText)]
        public string? Descrizione { get; set; }

        // ===================================
        // FILE
        // ===================================

        /// <summary>
        /// File da caricare
        /// </summary>
        [Display(Name = "File")]
        [Required(ErrorMessage = "Il file è obbligatorio")]
        public IFormFile? File { get; set; }

        // ===================================
        // SELECT LIST PER DROPDOWN
        // ===================================

        /// <summary>
        /// Lista tipi documento per dropdown
        /// </summary>
        public SelectList? TipiDocumentoSelectList { get; set; }

        // ===================================
        // INFO REGISTRO (READONLY)
        // ===================================

        /// <summary>
        /// Numero protocollo del registro (per display)
        /// </summary>
        public string? RegistroNumeroProtocollo { get; set; }

        /// <summary>
        /// Oggetto del registro (per display)
        /// </summary>
        public string? RegistroOggetto { get; set; }

        /// <summary>
        /// Ragione sociale del cliente (per display)
        /// </summary>
        public string? RegistroRagioneSociale { get; set; }

        // ===================================
        // CONFIGURAZIONE UPLOAD (popolata da appsettings)
        // ===================================

        /// <summary>
        /// Dimensione massima consentita per il file (in bytes)
        /// Popolato dal Service/Controller leggendo da appsettings
        /// </summary>
        public long MaxFileSize { get; set; }

        /// <summary>
        /// Estensioni consentite
        /// Popolato dal Service/Controller leggendo da appsettings
        /// </summary>
        public string[] AllowedExtensions { get; set; } = Array.Empty<string>();

        /// <summary>
        /// MIME types consentiti
        /// Popolato dal Service/Controller leggendo da appsettings
        /// </summary>
        public string[] AllowedMimeTypes { get; set; } = Array.Empty<string>();

        // ===================================
        // PROPRIETÀ CALCOLATE (per display)
        // ===================================

        /// <summary>
        /// Dimensione massima formattata per display
        /// </summary>
        public string MaxFileSizeFormattata
        {
            get
            {
                if (MaxFileSize == 0) return "N/D";

                string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                double len = MaxFileSize;
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
        /// Estensioni consentite come stringa (per display)
        /// </summary>
        public string AllowedExtensionsDisplay =>
            AllowedExtensions.Length > 0
                ? string.Join(", ", AllowedExtensions)
                : "N/D";

        /// <summary>
        /// Estensioni consentite per attributo accept HTML
        /// </summary>
        public string AllowedExtensionsAccept =>
            AllowedExtensions.Length > 0
                ? string.Join(",", AllowedExtensions)
                : "*/*";

        public IEnumerable<AllegatoRegistroListViewModel>? AllegatiEsistenti { get; set; }
    }

    /// <summary>
    /// ViewModel per l'upload multiplo di allegati
    /// </summary>
    public class AllegatoRegistroUploadMultiploViewModel
    {
        // ===================================
        // RIFERIMENTO REGISTRO
        // ===================================

        /// <summary>
        /// ID del registro contratti a cui allegare i file
        /// </summary>
        [Required(ErrorMessage = "Il registro è obbligatorio")]
        public Guid RegistroContrattiId { get; set; }

        // ===================================
        // TIPO DOCUMENTO DEFAULT
        // ===================================

        /// <summary>
        /// ID del tipo documento di default per tutti i file
        /// </summary>
        [Display(Name = "Tipo Documento")]
        [Required(ErrorMessage = "Il tipo documento è obbligatorio")]
        public Guid TipoDocumentoId { get; set; }

        // ===================================
        // FILES
        // ===================================

        /// <summary>
        /// Lista di file da caricare
        /// </summary>
        [Display(Name = "Files")]
        [Required(ErrorMessage = "Selezionare almeno un file")]
        public List<IFormFile>? Files { get; set; }

        // ===================================
        // SELECT LIST PER DROPDOWN
        // ===================================

        /// <summary>
        /// Lista tipi documento per dropdown
        /// </summary>
        public SelectList? TipiDocumentoSelectList { get; set; }

        // ===================================
        // INFO REGISTRO (READONLY)
        // ===================================

        /// <summary>
        /// Numero protocollo del registro (per display)
        /// </summary>
        public string? RegistroNumeroProtocollo { get; set; }

        /// <summary>
        /// Oggetto del registro (per display)
        /// </summary>
        public string? RegistroOggetto { get; set; }

        // ===================================
        // CONFIGURAZIONE UPLOAD (popolata da appsettings)
        // ===================================

        /// <summary>
        /// Numero massimo di file caricabili contemporaneamente
        /// Popolato dal Service/Controller leggendo da appsettings
        /// </summary>
        public int MaxFiles { get; set; }

        /// <summary>
        /// Dimensione massima consentita per singolo file (in bytes)
        /// Popolato dal Service/Controller leggendo da appsettings
        /// </summary>
        public long MaxFileSize { get; set; }

        /// <summary>
        /// Dimensione massima totale consentita (in bytes)
        /// Popolato dal Service/Controller leggendo da appsettings
        /// </summary>
        public long MaxTotalSize { get; set; }

        /// <summary>
        /// Estensioni consentite
        /// Popolato dal Service/Controller leggendo da appsettings
        /// </summary>
        public string[] AllowedExtensions { get; set; } = Array.Empty<string>();

        /// <summary>
        /// MIME types consentiti
        /// Popolato dal Service/Controller leggendo da appsettings
        /// </summary>
        public string[] AllowedMimeTypes { get; set; } = Array.Empty<string>();

        public IEnumerable<AllegatoRegistroListViewModel>? AllegatiEsistenti { get; set; }

        // ===================================
        // PROPRIETÀ CALCOLATE (per display)
        // ===================================

        /// <summary>
        /// Estensioni consentite per attributo accept HTML
        /// </summary>
        public string AllowedExtensionsAccept =>
            AllowedExtensions.Length > 0
                ? string.Join(",", AllowedExtensions)
                : "*/*";
    }
}