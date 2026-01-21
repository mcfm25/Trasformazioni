namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la visualizzazione del dettaglio di una categoria contratto
    /// </summary>
    public class CategoriaContrattoDetailsViewModel
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

        // ===== AUDIT =====

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

        // ===== PROPRIETÀ CALCOLATE =====

        /// <summary>
        /// Indica se la categoria può essere eliminata (non utilizzata)
        /// </summary>
        public bool CanDelete => NumeroUtilizzi == 0;

        /// <summary>
        /// Indica se la categoria può essere modificata
        /// </summary>
        public bool CanEdit => true;

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