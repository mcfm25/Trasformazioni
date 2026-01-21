namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità per la categorizzazione dei contratti e preventivi
    /// Gestita tramite CRUD dall'utente
    /// Esempi: Assistenza PDL, Hosting web, Nuovo sviluppo applicativo, ecc.
    /// </summary>
    public class CategoriaContratto : BaseEntity
    {
        /// <summary>
        /// Identificatore univoco
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome della categoria (es. "Assistenza PDL", "Hosting web")
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrizione opzionale della categoria
        /// </summary>
        public string? Descrizione { get; set; }

        /// <summary>
        /// Ordine di visualizzazione nelle liste
        /// </summary>
        public int Ordine { get; set; } = 0;

        /// <summary>
        /// Indica se la categoria è attiva e selezionabile
        /// </summary>
        public bool IsAttivo { get; set; } = true;

        // ===== NAVIGATION PROPERTIES =====

        /// <summary>
        /// Registri contratti che utilizzano questa categoria
        /// </summary>
        public virtual ICollection<RegistroContratti>? RegistriContratti { get; set; }
    }
}