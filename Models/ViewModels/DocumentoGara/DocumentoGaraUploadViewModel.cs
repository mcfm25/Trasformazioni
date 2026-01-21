using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.ViewModels.DocumentoGara
{
    /// <summary>
    /// ViewModel per il caricamento di un nuovo documento
    /// </summary>
    public class DocumentoGaraUploadViewModel
    {
        /// <summary>
        /// ID della gara a cui associare il documento (obbligatorio)
        /// </summary>
        [Required(ErrorMessage = "La gara è obbligatoria")]
        public Guid GaraId { get; set; }

        /// <summary>
        /// ID del lotto a cui associare il documento (opzionale)
        /// </summary>
        public Guid? LottoId { get; set; }

        /// <summary>
        /// ID del preventivo a cui associare il documento (opzionale)
        /// </summary>
        public Guid? PreventivoId { get; set; }

        /// <summary>
        /// ID dell'integrazione a cui associare il documento (opzionale)
        /// </summary>
        public Guid? IntegrazioneId { get; set; }

        ///// <summary>
        ///// Tipo di documento
        ///// </summary>
        //[Required(ErrorMessage = "Il tipo di documento è obbligatorio")]
        //public TipoDocumentoGara Tipo { get; set; }

        // DOPO
        /// <summary>
        /// ID del tipo documento (da tabella TipiDocumento)
        /// </summary>
        [Required(ErrorMessage = "Il tipo di documento è obbligatorio")]
        [Display(Name = "Tipo Documento")]
        public Guid TipoDocumentoId { get; set; }

        /// <summary>
        /// Descrizione opzionale del documento
        /// </summary>
        [StringLength(500, ErrorMessage = "La descrizione non può superare i 500 caratteri")]
        public string? Descrizione { get; set; }

        // Il file viene passato separatamente tramite IFormFile nel controller
    }
}