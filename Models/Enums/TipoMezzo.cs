using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.Enums
{
    /// <summary>
    /// Tipologia di mezzo aziendale
    /// </summary>
    public enum TipoMezzo
    {
        [Display(Name = "Automobile")]
        Auto = 1,

        [Display(Name = "Furgone")]
        Furgone = 2,

        [Display(Name = "Camion")]
        Camion = 3,

        [Display(Name = "Motociclo")]
        Moto = 4,

        [Display(Name = "Altro")]
        Altro = 99
    }
}