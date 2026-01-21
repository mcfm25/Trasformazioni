using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.Enums
{
    /// <summary>
    /// Stato operativo del mezzo
    /// </summary>
    public enum StatoMezzo
    {
        [Display(Name = "Disponibile")]
        Disponibile = 1,

        [Display(Name = "In Uso")]
        InUso = 2,

        [Display(Name = "In Manutenzione")]
        InManutenzione = 3,

        [Display(Name = "Dismesso")]
        Dismesso = 4,

        [Display(Name = "Prenotato")]
        Prenotato = 5
    }
}