using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.Enums
{
    /// <summary>
    /// Tipologie di lotto in base all'oggetto della fornitura
    /// </summary>
    public enum TipologiaLotto
    {
        /// <summary>
        /// Fornitura di hardware e apparecchiature
        /// </summary>
        [Display(Name = "Fornitura Hardware")]
        FornituraHardware,

        /// <summary>
        /// Prestazione di servizi
        /// </summary>
        [Display(Name = "Servizi")]
        Servizi,

        /// <summary>
        /// Servizi di manutenzione e assistenza
        /// </summary>
        [Display(Name = "Manutenzione")]
        Manutenzione,

        /// <summary>
        /// Lavori di costruzione o ristrutturazione
        /// </summary>
        [Display(Name = "Lavori")]
        Lavori,

        /// <summary>
        /// Lotto con oggetto misto (hardware + servizi)
        /// </summary>
        [Display(Name = "Misto")]
        Misto,

        /// <summary>
        /// Altra tipologia non classificata
        /// </summary>
        [Display(Name = "Altro")]
        Altro
    }
}