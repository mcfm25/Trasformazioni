using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.Enums
{
    /// <summary>
    /// Stati possibili per un Preventivo
    /// Include gestione scadenze e auto-rinnovo
    /// </summary>
    public enum StatoPreventivo
    {
        /// <summary>
        /// Preventivo richiesto ma non ancora ricevuto
        /// </summary>
        [Display(Name = "In Attesa")]
        InAttesa,

        /// <summary>
        /// Preventivo ricevuto dal fornitore
        /// </summary>
        [Display(Name = "Ricevuto")]
        Ricevuto,

        /// <summary>
        /// Preventivo valido e utilizzabile
        /// </summary>
        [Display(Name = "Valido")]
        Valido,

        /// <summary>
        /// Preventivo in scadenza (entro GiorniPreavviso dalla DataScadenza)
        /// </summary>
        [Display(Name = "In Scadenza")]
        InScadenza,

        /// <summary>
        /// Preventivo scaduto (oltre DataScadenza)
        /// </summary>
        [Display(Name = "Scaduto")]
        Scaduto,

        /// <summary>
        /// Preventivo rinnovato automaticamente (HasAutoRinnovo = true)
        /// </summary>
        [Display(Name = "Rinnovato")]
        Rinnovato,

        /// <summary>
        /// Preventivo annullato manualmente
        /// </summary>
        [Display(Name = "Annullato")]
        Annullato
    }
}