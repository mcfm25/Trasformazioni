using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Rappresenta un tipo di documento configurabile dall'utente.
    /// Ogni tipo è associato ad un'area specifica dell'applicativo.
    /// I tipi di sistema (IsSystem = true) non possono essere eliminati.
    /// </summary>
    public class TipoDocumento : BaseEntity
    {
        /// <summary>
        /// Identificativo univoco del tipo documento
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome del tipo documento (univoco per area)
        /// </summary>
        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrizione opzionale del tipo documento
        /// </summary>
        [StringLength(500, ErrorMessage = "La descrizione non può superare i 500 caratteri")]
        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }

        /// <summary>
        /// Area dell'applicativo a cui appartiene questo tipo
        /// </summary>
        [Required(ErrorMessage = "L'area è obbligatoria")]
        [Display(Name = "Area")]
        public AreaDocumento Area { get; set; }

        /// <summary>
        /// Indica se è un tipo di sistema (non eliminabile).
        /// I tipi di sistema vengono creati automaticamente dalla migrazione
        /// dei valori dell'enum TipoDocumentoGara.
        /// </summary>
        [Display(Name = "Tipo di Sistema")]
        public bool IsSystem { get; set; } = false;

        /// <summary>
        /// Codice di riferimento per i tipi di sistema.
        /// Corrisponde al valore dell'enum (es. TipoDocumentoGara) per controlli nel workflow.
        /// NULL per i tipi personalizzati creati dall'utente.
        /// Immutabile per i tipi IsSystem.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Codice Riferimento")]
        public string? CodiceRiferimento { get; set; }

        // ===== NAVIGATION PROPERTIES =====

        /// <summary>
        /// Documenti di gara che utilizzano questo tipo
        /// </summary>
        public virtual ICollection<DocumentoGara>? DocumentiGara { get; set; }

        // Future: DocumentiLotto, DocumentiMezzo, ecc.
    }
}