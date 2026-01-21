namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la visualizzazione in lista delle categorie contratto
    /// </summary>
    public class CategoriaContrattoListViewModel
    {
        /// <summary>
        /// Identificatore univoco
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome della categoria
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrizione della categoria
        /// </summary>
        public string? Descrizione { get; set; }

        /// <summary>
        /// Ordine di visualizzazione
        /// </summary>
        public int Ordine { get; set; }

        /// <summary>
        /// Indica se la categoria è attiva
        /// </summary>
        public bool IsAttivo { get; set; }

        /// <summary>
        /// Numero di registri che utilizzano questa categoria
        /// </summary>
        public int NumeroUtilizzi { get; set; }

        // ===== PROPRIETÀ CALCOLATE =====

        /// <summary>
        /// Indica se la categoria può essere eliminata (non utilizzata)
        /// </summary>
        public bool CanDelete => NumeroUtilizzi == 0;

        /// <summary>
        /// Badge CSS per lo stato attivo/inattivo
        /// </summary>
        public string StatoBadgeClass => IsAttivo
            ? "badge bg-success"
            : "badge bg-secondary";

        /// <summary>
        /// Testo per lo stato attivo/inattivo
        /// </summary>
        public string StatoDescrizione => IsAttivo
            ? "Attiva"
            : "Disattivata";
    }
}