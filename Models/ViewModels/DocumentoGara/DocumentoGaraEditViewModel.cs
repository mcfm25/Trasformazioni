using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.ViewModels.DocumentoGara
{
    /// <summary>
    /// ViewModel per la modifica di un documento esistente
    /// Permette solo di modificare tipo e descrizione, NON il file fisico
    /// </summary>
    public class DocumentoGaraEditViewModel
    {
        public Guid Id { get; set; }

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
        /// Descrizione del documento
        /// </summary>
        [StringLength(500, ErrorMessage = "La descrizione non può superare i 500 caratteri")]
        public string? Descrizione { get; set; }

        // Campi read-only per visualizzazione
        public string NomeFile { get; set; } = string.Empty;
        public long DimensioneBytes { get; set; }
        public string DimensioneFormatted { get; set; } = string.Empty;
        public DateTime DataCaricamento { get; set; }
        public string CaricatoDaNome { get; set; } = string.Empty;
    }
}