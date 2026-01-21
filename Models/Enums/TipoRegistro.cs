using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.Enums
{
    /// <summary>
    /// Tipologia di documento nel Registro Contratti
    /// Distingue tra Preventivi emessi e Contratti stipulati
    /// </summary>
    public enum TipoRegistro
    {
        /// <summary>
        /// Preventivo emesso verso il cliente
        /// </summary>
        [Display(Name = "Preventivo")]
        Preventivo = 0,

        /// <summary>
        /// Contratto stipulato con il cliente
        /// </summary>
        [Display(Name = "Contratto")]
        Contratto = 1
    }
}