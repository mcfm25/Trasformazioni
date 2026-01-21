using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.Enums
{
    /// <summary>
    /// Motivo per cui un mezzo viene assegnato a un utente
    /// </summary>
    public enum MotivoAssegnazione
    {
        [Display(Name = "Uso Quotidiano")]
        UsoQuotidiano = 1,

        [Display(Name = "Trasferta")]
        Trasferta = 2,

        [Display(Name = "Manutenzione")]
        Manutenzione = 3,

        [Display(Name = "Emergenza")]
        Emergenza = 4,

        [Display(Name = "Altro")]
        Altro = 99
    }
}