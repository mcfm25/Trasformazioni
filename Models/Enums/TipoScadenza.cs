using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.Enums
{
    /// <summary>
    /// Tipologie di scadenze gestite dal sistema
    /// Utilizzate per scadenzario automatico e notifiche
    /// </summary>
    public enum TipoScadenza
    {
        /// <summary>
        /// Scadenza per la presentazione dell'offerta
        /// </summary>
        [Display(Name = "Presentazione Offerta")]
        PresentazioneOfferta,

        /// <summary>
        /// Termine ultimo per richiedere chiarimenti all'ente
        /// </summary>
        [Display(Name = "Richiesta Chiarimenti")]
        RichiestaChiarimenti,

        /// <summary>
        /// Scadenza di validità di un preventivo
        /// </summary>
        [Display(Name = "Scadenza Preventivo")]
        ScadenzaPreventivo,

        /// <summary>
        /// Scadenza per l'integrazione documentale richiesta dall'ente
        /// </summary>
        [Display(Name = "Integrazione Documentazione")]
        IntegrazioneDocumentazione,

        /// <summary>
        /// Data prevista per la stipula del contratto
        /// </summary>
        [Display(Name = "Stipula Contratto")]
        StipulaContratto,

        /// <summary>
        /// Data di scadenza del contratto
        /// </summary>
        [Display(Name = "Scadenza Contratto")]
        ScadenzaContratto,

        /// <summary>
        /// Altra tipologia di scadenza non classificata
        /// </summary>
        [Display(Name = "Altro")]
        Altro
    }
}