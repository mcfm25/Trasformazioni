namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Classe base per tutte le entità del dominio.
    /// Fornisce proprietà comuni per audit trail e soft delete.
    /// </summary>
    public abstract class BaseEntity : IAuditableEntity
    {
        /// <summary>
        /// Data e ora di creazione dell'entità (UTC)
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Identificatore dell'utente che ha creato l'entità
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Data e ora dell'ultima modifica dell'entità (UTC)
        /// </summary>
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// Identificatore dell'utente che ha modificato per ultimo l'entità
        /// </summary>
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Data e ora di cancellazione logica dell'entità (UTC)
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Identificatore dell'utente che ha cancellato logicamente l'entità
        /// </summary>
        public string? DeletedBy { get; set; }

        /// <summary>
        /// Indica se l'entità è stata cancellata logicamente
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
}