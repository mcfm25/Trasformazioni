using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.Enums
{
    /// <summary>
    /// Stati possibili per una Gara
    /// Lo stato è derivato automaticamente dagli stati dei Lotti
    /// </summary>
    public enum StatoGara
    {
        /// <summary>
        /// Nessun lotto o tutti i lotti in Bozza
        /// </summary>
        [Display(Name = "Bozza")]
        Bozza,

        /// <summary>
        /// Almeno un lotto in valutazione/elaborazione/presentazione
        /// </summary>
        [Display(Name = "In Lavorazione")]
        InLavorazione,

        /// <summary>
        /// Tutti i lotti hanno raggiunto un esito finale (Vinto/Perso/Scartato/Rifiutato)
        /// </summary>
        [Display(Name = "Conclusa")]
        Conclusa,

        /// <summary>
        /// Gara chiusa manualmente dall'amministratore
        /// </summary>
        [Display(Name = "Chiusa Manualmente")]
        ChiusaManualmente
    }
}