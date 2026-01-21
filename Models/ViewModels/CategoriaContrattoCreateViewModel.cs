using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la creazione di una nuova categoria contratto
    /// </summary>
    public class CategoriaContrattoCreateViewModel
    {
        /// <summary>
        /// Nome della categoria
        /// </summary>
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrizione della categoria
        /// </summary>
        [Display(Name = "Descrizione")]
        [StringLength(500, ErrorMessage = "La descrizione non può superare i 500 caratteri")]
        [DataType(DataType.MultilineText)]
        public string? Descrizione { get; set; }

        /// <summary>
        /// Ordine di visualizzazione
        /// </summary>
        [Display(Name = "Ordine")]
        [Range(0, 9999, ErrorMessage = "L'ordine deve essere compreso tra 0 e 9999")]
        public int Ordine { get; set; } = 0;

        /// <summary>
        /// Indica se la categoria è attiva
        /// </summary>
        [Display(Name = "Attiva")]
        public bool IsAttivo { get; set; } = true;
    }
}