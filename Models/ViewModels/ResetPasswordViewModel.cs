using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per il reset della password
    /// </summary>
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria")]
        [StringLength(100, ErrorMessage = "La {0} deve essere lunga almeno {2} caratteri.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nuova Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Conferma Password")]
        [Compare("Password", ErrorMessage = "Le password non corrispondono.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Token di reset generato dal sistema
        /// </summary>
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
