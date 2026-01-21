using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    // =============================================
    // LIST VIEW MODEL
    // =============================================

    /// <summary>
    /// ViewModel per la lista dei tipi documento
    /// </summary>
    public class TipoDocumentoListViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }

        [Display(Name = "Area")]
        public AreaDocumento Area { get; set; }

        [Display(Name = "Area")]
        public string AreaDisplayName { get; set; } = string.Empty;

        [Display(Name = "Di Sistema")]
        public bool IsSystem { get; set; }

        [Display(Name = "Documenti Associati")]
        public int NumeroDocumenti { get; set; }

        [Display(Name = "Data Creazione")]
        public DateTime CreatedAt { get; set; }
    }

    // =============================================
    // DETAILS VIEW MODEL
    // =============================================

    /// <summary>
    /// ViewModel per il dettaglio di un tipo documento
    /// </summary>
    public class TipoDocumentoDetailsViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }

        [Display(Name = "Area")]
        public AreaDocumento Area { get; set; }

        [Display(Name = "Area")]
        public string AreaDisplayName { get; set; } = string.Empty;

        [Display(Name = "Tipo di Sistema")]
        public bool IsSystem { get; set; }

        [Display(Name = "Documenti Associati")]
        public int NumeroDocumenti { get; set; }

        [Display(Name = "Può essere eliminato")]
        public bool CanDelete { get; set; }

        [Display(Name = "Può essere modificato")]
        public bool CanEdit { get; set; }

        // Audit
        [Display(Name = "Data Creazione")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Creato Da")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Ultima Modifica")]
        public DateTime? ModifiedAt { get; set; }

        [Display(Name = "Modificato Da")]
        public string? ModifiedBy { get; set; }
    }

    // =============================================
    // CREATE VIEW MODEL
    // =============================================

    /// <summary>
    /// ViewModel per la creazione di un tipo documento
    /// </summary>
    public class TipoDocumentoCreateViewModel
    {
        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descrizione non può superare i 500 caratteri")]
        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }

        [Required(ErrorMessage = "L'area è obbligatoria")]
        [Display(Name = "Area")]
        public AreaDocumento Area { get; set; }
    }

    // =============================================
    // EDIT VIEW MODEL
    // =============================================

    /// <summary>
    /// ViewModel per la modifica di un tipo documento
    /// </summary>
    public class TipoDocumentoEditViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descrizione non può superare i 500 caratteri")]
        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }

        [Required(ErrorMessage = "L'area è obbligatoria")]
        [Display(Name = "Area")]
        public AreaDocumento Area { get; set; }

        [Display(Name = "Tipo di Sistema")]
        public bool IsSystem { get; set; }

        /// <summary>
        /// Indica se l'area può essere modificata (false se ci sono documenti associati)
        /// </summary>
        public bool CanChangeArea { get; set; } = true;
    }

    // =============================================
    // DROPDOWN VIEW MODEL
    // =============================================

    /// <summary>
    /// ViewModel per le dropdown di selezione tipo documento
    /// </summary>
    public class TipoDocumentoDropdownViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public AreaDocumento Area { get; set; }
        public bool IsSystem { get; set; }

        /// <summary>
        /// Testo da visualizzare nella dropdown (Nome + eventuale indicatore sistema)
        /// </summary>
        public string DisplayText => IsSystem ? $"{Nome} (Sistema)" : Nome;
    }

    // =============================================
    // FILTER VIEW MODEL
    // =============================================

    /// <summary>
    /// ViewModel per i filtri della lista tipi documento
    /// </summary>
    public class TipoDocumentoFilterViewModel
    {
        [Display(Name = "Cerca")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Area")]
        public AreaDocumento? Area { get; set; }

        [Display(Name = "Solo Tipi di Sistema")]
        public bool? SoloSistema { get; set; }

        [Display(Name = "Solo Personalizzati")]
        public bool? SoloPersonalizzati { get; set; }
    }
}
