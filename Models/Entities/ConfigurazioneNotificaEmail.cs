namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Configurazione per le notifiche email di una specifica operazione
    /// </summary>
    public class ConfigurazioneNotificaEmail : BaseEntity
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Codice univoco operazione (es. "CONTRATTO_IN_SCADENZA")
        /// </summary>
        public string Codice { get; set; } = string.Empty;

        /// <summary>
        /// Descrizione leggibile per l'UI
        /// </summary>
        public string Descrizione { get; set; } = string.Empty;

        /// <summary>
        /// Modulo di appartenenza (es. "RegistroContratti", "Gare", "Mezzi")
        /// </summary>
        public string Modulo { get; set; } = string.Empty;

        /// <summary>
        /// Se false, le notifiche per questa operazione sono disabilitate
        /// </summary>
        public bool IsAttiva { get; set; } = true;

        /// <summary>
        /// Oggetto email di default (può contenere placeholder come {NumeroProtocollo})
        /// </summary>
        public string? OggettoEmailDefault { get; set; }

        /// <summary>
        /// Note amministrative
        /// </summary>
        public string? Note { get; set; }

        // ===================================
        // NAVIGATION PROPERTIES
        // ===================================

        /// <summary>
        /// Lista dei destinatari configurati per questa operazione
        /// </summary>
        public virtual ICollection<DestinatarioNotificaEmail> Destinatari { get; set; } = new List<DestinatarioNotificaEmail>();

        // ===================================
        // COMPUTED PROPERTIES
        // ===================================

        /// <summary>
        /// Numero di destinatari attivi
        /// </summary>
        public int DestinatariCount => Destinatari?.Count(d => !d.IsDeleted) ?? 0;
    }
}