namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità per la gestione dei reparti aziendali
    /// </summary>
    public class Reparto : BaseEntity
    {
        /// <summary>
        /// Identificatore univoco
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome del reparto
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Email del reparto (obbligatoria)
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Descrizione del reparto (opzionale)
        /// </summary>
        public string? Descrizione { get; set; }

        // ===================================
        // NAVIGATION PROPERTIES
        // ===================================

        /// <summary>
        /// Utenti appartenenti al reparto
        /// </summary>
        public virtual ICollection<ApplicationUser> Utenti { get; set; } = new List<ApplicationUser>();
    }
}