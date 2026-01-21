using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Destinatario di una notifica email
    /// Può essere un Reparto (1 email), un Ruolo (N email) o un Utente specifico (1 email)
    /// </summary>
    public class DestinatarioNotificaEmail : BaseEntity
    {
        public Guid Id { get; set; }

        /// <summary>
        /// FK alla configurazione
        /// </summary>
        public Guid ConfigurazioneNotificaEmailId { get; set; }

        /// <summary>
        /// Tipo di destinatario
        /// </summary>
        public TipoDestinatarioNotifica Tipo { get; set; }

        /// <summary>
        /// Se Tipo = Reparto: ID del reparto (risolve a Reparto.Email)
        /// </summary>
        public Guid? RepartoId { get; set; }

        /// <summary>
        /// Se Tipo = Ruolo: nome del ruolo (risolve a tutti gli utenti attivi con quel ruolo)
        /// </summary>
        public string? Ruolo { get; set; }

        /// <summary>
        /// Se Tipo = Utente: ID dell'utente specifico
        /// </summary>
        public string? UtenteId { get; set; }

        /// <summary>
        /// Ordine di visualizzazione
        /// </summary>
        public int Ordine { get; set; } = 0;

        /// <summary>
        /// Note/descrizione (es. "Responsabile progetto")
        /// </summary>
        public string? Note { get; set; }

        // ===================================
        // NAVIGATION PROPERTIES
        // ===================================

        public virtual ConfigurazioneNotificaEmail ConfigurazioneNotificaEmail { get; set; } = null!;
        public virtual Reparto? Reparto { get; set; }
        public virtual ApplicationUser? Utente { get; set; }

        // ===================================
        // COMPUTED PROPERTIES
        // ===================================

        /// <summary>
        /// Descrizione leggibile del destinatario per l'UI
        /// </summary>
        public string DescrizioneDestinatario => Tipo switch
        {
            TipoDestinatarioNotifica.Reparto => $"Reparto: {Reparto?.Nome ?? "N/D"}",
            TipoDestinatarioNotifica.Ruolo => $"Ruolo: {Ruolo}",
            TipoDestinatarioNotifica.Utente => $"Utente: {Utente?.NomeCompleto ?? "N/D"}",
            _ => "N/D"
        };
    }
}