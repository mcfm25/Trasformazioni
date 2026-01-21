using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.Enums
{
    /// <summary>
    /// Tipo di proprietà del mezzo (proprietà aziendale o noleggio)
    /// </summary>
    public enum TipoProprietaMezzo
    {
        [Display(Name = "Proprietà")]
        Proprieta = 1,

        [Display(Name = "Noleggio")]
        Noleggio = 2
    }
}