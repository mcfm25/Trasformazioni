using Microsoft.AspNetCore.Identity;

namespace Trasformazioni.Models.Entities
{
    public class ApplicationUser : IdentityUser, IAuditableEntity
    {
        public string Nome { get; set; } = string.Empty;
        public string Cognome { get; set; } = string.Empty;

        /// <summary>
        /// FK al reparto di appartenenza
        /// </summary>
        public Guid? RepartoId { get; set; }

        public DateTime DataAssunzione { get; set; }
        public bool IsAttivo { get; set; } = true;

        // Campi audit (opzionale - solo se necessario tracciare modifiche utente)
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }

        // Soft delete (opzionale)
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

        // ===================================
        // NAVIGATION PROPERTIES
        // ===================================

        /// <summary>
        /// Reparto di appartenenza
        /// </summary>
        public virtual Reparto? Reparto { get; set; }

        /// <summary>
        /// Collezione di tutte le assegnazioni mezzi dell'utente (storico completo)
        /// </summary>
        public virtual ICollection<AssegnazioneMezzo> AssegnazioniMezzi { get; set; } = new List<AssegnazioneMezzo>();

        // ===================================
        // Proprietà calcolate
        // ===================================

        public string NomeCompleto => $"{Nome} {Cognome}";

        /// <summary>
        /// Nome del reparto (per retrocompatibilità)
        /// </summary>
        public string? RepartoNome => Reparto?.Nome;
    }
}