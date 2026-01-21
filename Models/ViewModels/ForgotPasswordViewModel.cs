using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la richiesta di recupero password
    /// </summary>
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }
}
